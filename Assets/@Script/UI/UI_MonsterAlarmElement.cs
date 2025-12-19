using DG.Tweening;
using UnityEngine;

namespace MewVivor.UISubItemElement
{
    public class UI_MonsterAlarmElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0;
            Sequence seq = DOTween.Sequence();
            seq.Append(_canvasGroup.DOFade(1, 0.5f))
                .Append(_canvasGroup.DOFade(0, 0.5f))
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(Hide);
        }

        public void Hide()
        {
            _canvasGroup.alpha = 1;
            gameObject.SetActive(false);
        }
    }
}