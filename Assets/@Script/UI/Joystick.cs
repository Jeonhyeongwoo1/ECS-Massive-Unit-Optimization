using System;
using System.Collections;
using MewVivor.InGame.Input;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MewVivor.InGame.View
{
    public struct PlayerInputComponent : IComponentData
    {
        public float2 Movement;
        public bool IsMoving;
    }
    
    public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _bgRectTransform;
        [SerializeField] private RectTransform _handlerRectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Vector2 _inputVector;
        private Coroutine _fadeCor;

        private EntityManager _entityManager;
        private Unity.Entities.Entity _playerInputEntity;

        private void OnEnable()
        {
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
        }

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = _entityManager.CreateEntityQuery(typeof(PlayerInputComponent));
            if (!query.IsEmpty)
            {
                _playerInputEntity = query.GetSingletonEntity();
            }
            else
            {
                _playerInputEntity = _entityManager.CreateEntity(typeof(PlayerInputComponent));
            }

            InputHandler.onActivateInputHandlerAction += OnActivate;
        }

        private void OnDestroy()
        {
            InputHandler.onActivateInputHandlerAction -= OnActivate;
        }

        private void OnDisable()
        {
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
                _fadeCor = null;
            }
        }

        private void OnActivate(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        // 드래그 시 호출
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            // 배경 안에서 핸들의 위치 계산
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _bgRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out pos
            );

            pos.x = (pos.x / _bgRectTransform.sizeDelta.x);
            pos.y = (pos.y / _bgRectTransform.sizeDelta.y);

            // _handlerRectTransform.anchoredPosition = pos;
            _inputVector = new Vector2(pos.x * 2, pos.y * 2);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            // Debug.Log(_inputVector);
            _handlerRectTransform.anchoredPosition = _bgRectTransform.anchoredPosition + new Vector2(
                _inputVector.x * (_bgRectTransform.sizeDelta.x / 2),
                _inputVector.y * (_bgRectTransform.sizeDelta.y / 2)
            );

            // _entityManager.SetComponentData(_playerInputEntity,
            //     new PlayerInputComponent
            //     {
            //         Movement = _inputVector,
            //         IsMoving = _inputVector.x != 0 || _inputVector.y != 0
            //     });

            InputHandler.onInputAction?.Invoke(_inputVector);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _bgRectTransform.position = eventData.position;
            OnDrag(eventData);
            
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
            
            _fadeCor = StartCoroutine(DoFadeCor(true, 0.1f));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
            
            _fadeCor = StartCoroutine(DoFadeCor(false, 0.3f));
            // InputHandler.onPointerUpAction?.Invoke(Vector2.zero);
            InputHandler.onInputAction?.Invoke(Vector2.zero);
        }

        private IEnumerator DoFadeCor(bool isFadeOut, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
             
                float endValue = isFadeOut ? 1 : 0;
                float starValue = isFadeOut ? 0 : 1;

                _canvasGroup.alpha = Mathf.Lerp(starValue, endValue, elapsed / duration);
                yield return null;
            }
        }
    }
}
















