using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MewVivor.Util
{
    public class OnClickScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsOn = true;
        
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _onEnterScale = new Vector3(0.85f,0.85f,0.85f);
        [SerializeField] private Vector3 _onExitScale = new Vector3(1, 1, 1);
        [SerializeField] private float _duration = 0.15f; 
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsOn)
            {
                return;
            }
            
            if (_target == null)
            {
                transform.DOKill();
                transform.DOScale(_onEnterScale, _duration);
            }
            else
            {
                _target.DOKill();
                _target.DOScale(_onEnterScale, _duration);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsOn)
            {
                return;
            }
            
            if (_target == null)
            {
                transform.DOKill();
                transform.DOScale(_onExitScale, _duration);
            }
            else
            {
                _target.DOKill();
                _target.DOScale(_onExitScale, _duration);
            }
        }
    }
}