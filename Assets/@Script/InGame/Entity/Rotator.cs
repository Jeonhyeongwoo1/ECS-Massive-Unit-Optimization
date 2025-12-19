using System;
using DG.Tweening;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;
        
        private float _rotationSpeed;

        private void OnEnable()
        {
            _rotationSpeed = 360f / duration; // 초당 몇 도 회전할지 계산
        }

        private void Update()
        {
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }
    }
}