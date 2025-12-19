using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UI
{
    public class UI_CurrencyObject : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private AnimationCurve _scaleDownAnimationCurve;

        private Sequence _seq;
        public void Show(Sprite currencySprite, Vector3 spawnPoint,  Vector3 endPoint, Action onCallback)
        {
            _image.sprite = currencySprite;
            _image.SetNativeSize();
            transform.position = spawnPoint;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            if (_seq != null)
            {
                _seq.Kill();
            }
            
            _seq = DOTween.Sequence();
            _seq.Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
            _seq.Append(DOTween.Sequence()
                .Join(transform.DOMove(endPoint, 0.5f))
                .Join(transform.DOScale(Vector3.one * 0.7f, 0.5f)))
                .OnComplete(()=>
                {
                    onCallback?.Invoke();
                });
        }

        public void ReleaseObject()
        {
            if (_seq != null)
            {
                _seq.Kill();
            }
            
            Manager.I.Pool.ReleaseObject(nameof(UI_CurrencyObject), gameObject);
        }
    }
}