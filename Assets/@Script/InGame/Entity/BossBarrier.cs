using System;
using DG.Tweening;
using MewVivor.Data;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class BossBarrier : MonoBehaviour
    {
        [SerializeField] private Transform _barrierImageTransform;
        
        private bool _isEnteredPlayer = false;
        private float _elapsed = 0;
        private float _interval = 0.5f;
        private bool _isPossibleTakeDamage = false;

        public void Initialize(Transform parent, Vector3 spawnPosition)
        {
            _isPossibleTakeDamage = false;
            transform.SetParent(parent);
            transform.position = spawnPosition;
            _barrierImageTransform.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        
        public void SpawnAnimation()
        {
            _isEnteredPlayer = false;
            _isPossibleTakeDamage = true;
            _barrierImageTransform.gameObject.SetActive(true);
            _barrierImageTransform.localScale = new Vector3(1, 0, 1);
            _barrierImageTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                _isEnteredPlayer = true;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                _isEnteredPlayer = false;
            }   
        }

        private void Update()
        {
            if (!_isEnteredPlayer || !_isPossibleTakeDamage)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed < _interval)
            {
                return;
            }
            
            _elapsed = 0;
            Manager.I.Object.Player.TakeDamage(Const.WALL_COLLISION_TAKE_DAMAGE, null);
        }

        public void Release()
        {
            Manager.I.Pool.ReleaseObject(nameof(BossBarrier), gameObject);
        }
    }
}