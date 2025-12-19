using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_MergeItem : UI_SubItemElement
    {
        [SerializeField] private Image _gradeImage;
        [SerializeField] private Image _equipmentTypeImage;
        [SerializeField] private Image _equipImage;
        [SerializeField] private GameObject _equipGradeNumberObject;
        [SerializeField] private TextMeshProUGUI _equipGradeNumberText;
        [SerializeField] private RectTransform _rect;
        private Vector3 _originPos;
        
        public void UpdateUI(Sprite gradeSprite, Sprite equipmentSprite, Sprite equipSprite, Vector3 scale,
            int gradeCount = 0)
        {
            _gradeImage.sprite = gradeSprite;
            _equipmentTypeImage.sprite = equipmentSprite;
            _equipImage.sprite = equipSprite;

            _equipGradeNumberObject.SetActive(gradeCount > 0);
            _equipGradeNumberText.text = gradeCount.ToString();
            transform.localScale = scale;
            gameObject.SetActive(true);
        }

        public IEnumerator MergeAnimationCor(Vector3 endPosition, Action callback)
        {
            _originPos = transform.position;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOShakePosition(1, strength: 0.5f, vibrato: 10, randomness: 90f));
            sequence.Append(transform.DOMove(endPosition, 0.5f).SetEase(Ease.InOutSine))
                .Join(transform.DOScale(Vector3.zero, 0.5f));
            // yield return sequence.WaitForCompletion();

            yield return new WaitForSeconds(1.5f);
            callback.Invoke();
            transform.position = _originPos;
        }
    }
}