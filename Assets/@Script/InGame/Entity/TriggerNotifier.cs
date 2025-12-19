using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class TriggerNotifier : MonoBehaviour
    {
        private Action<Transform> _onTriggerEnterAction;
        private Action<Transform> _onTriggerExitAction;
        private string _tag;
        
        public void Initialize(string tag, Action<Transform> onTriggerEnterAction, Action<Transform> onTriggerExitAction)
        {
            _tag = tag;
            _onTriggerEnterAction = onTriggerEnterAction;
            _onTriggerExitAction = onTriggerExitAction;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox))
            {
                _onTriggerEnterAction?.Invoke(other.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox))
            {
                _onTriggerExitAction?.Invoke(other.transform);
            }
        }

        public void Release()
        {
            gameObject.SetActive(false);
        }

        public void DoScaleAnimation()
        {
            transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                gameObject.SetActive(false);
            });
        }
    }
}