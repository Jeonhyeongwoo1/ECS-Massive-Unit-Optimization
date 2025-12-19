using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextBlinker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _targetText;

    private Sequence _seq;
    
    private void OnEnable()
    {
        if (_targetText == null)
        {
            TryGetComponent(out _targetText);
        }
        
        _seq = DOTween.Sequence();
        // 알파를 1 → 0.2 → 1로 3초 동안 자연스럽게 반복
        _seq.Append(_targetText.DOFade(0.2f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));
    }

    private void OnDisable()
    {
        if (_seq != null)
        {
            _seq.Kill();
            _seq = null;
        }
    }
}
