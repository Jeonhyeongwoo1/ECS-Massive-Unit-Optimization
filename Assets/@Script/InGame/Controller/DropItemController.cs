using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Managers;
using UnityEngine;
using UnityEngine.Serialization;

//죽었을 때, 새로운 웨이브가 시작되었을때 스폰
namespace MewVivor.InGame.Controller
{
    public abstract class DropItemController : MonoBehaviour
    {
        public DropableItemType DropableItemType => _dropableItemType;
        public DropItemData DropItemData => _dropItemData;
        
        [SerializeField] protected DropableItemType _dropableItemType;
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        protected bool IsRelease { get; set; }
        
        private DropItemData _dropItemData;
        private CancellationTokenSource _moveToTargetCts;
        
        //스폰, 먹었을 때 처리
        public virtual void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            _dropItemData = dropItemData;
            transform.position = spawnPosition;
            IsRelease = false;
            gameObject.SetActive(true);

            var sprite = Manager.I.Resource.Load<Sprite>(_dropItemData.SpriteName);
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = Const.SortinglayerOrder_Gem;
        }

        private void OnDestroy()
        {
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
        }

        public virtual void Release()
        {
            if (IsRelease)
            {
                return;
            }
            
            string name = null;
            switch (_dropableItemType)
            {
                case DropableItemType.Gem:
                    name = Const.ExpGem;
                    break;
                case DropableItemType.ItemBox:
                case DropableItemType.Magnet:
                case DropableItemType.Bomb:
                case DropableItemType.Potion:
                case DropableItemType.SkillUp:
                case DropableItemType.Gold:
                case DropableItemType.Jewel:
                default:
                    name = _dropableItemType.ToString();
                    break;
            }
            
            IsRelease = true;
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
            Manager.I.Pool.ReleaseObject(name, gameObject);
            Manager.I.Object.RemoveDropItem(this);
        }

        public virtual void GetItem(Transform target, bool isMagnet = false, Action callback = null)
        {
            if (_moveToTargetCts != null)
            {
                return;
            }
            
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
            MoveToTargetAsync(target, isMagnet, callback).Forget();
        }
        
        protected async UniTaskVoid MoveToTargetAsync(Transform target, bool isMagnet, Action callback)
        {
            if (IsRelease)
            {
                return;
            }

            const float speedValue = 30;
            _moveToTargetCts = new CancellationTokenSource();
            CancellationToken token = _moveToTargetCts.Token;
            float speed = isMagnet ? speedValue * 2 : speedValue;
            while (!IsRelease)
            {
                Vector3 position = transform.position;
                transform.position = Vector3.MoveTowards(position, target.position, Time.deltaTime * speed);

                if (Vector2.Distance(transform.position, target.position) < 1.5f)
                {
                    break;
                }
                
                try
                {
                    await UniTask.Yield(cancellationToken: token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogError($"{nameof(MoveToTargetAsync)} error {e.Message}");
                }
            }

            CompletedGetItem();
            callback?.Invoke();
        }
        
        protected virtual void CompletedGetItem()
        {
            Release();
            
            if (_dropItemData != null)
            {
                Manager.I.Event.Raise(GameEventType.ActivateDropItem, this);
            }
        }
    }
}