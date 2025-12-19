using System;
using DG.Tweening;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class BossAndEliteSpawnAlarmCircle : MonoBehaviour
    {
        // [SerializeField] private SpriteRenderer _sprite;

        public void Show(Vector3 spawnPosition, Action callback)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            DOVirtual.DelayedCall(3, callback.Invoke);
            // _sprite.DOKill();
            // _sprite.DOFade(0f, 0.5f)
            //     .SetLoops(6, LoopType.Yoyo)
            //     .SetEase(Ease.InOutSine)
            //     .OnComplete(callback.Invoke);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnDisable()
        {
            // _sprite.DOKill();
        }
    }
}