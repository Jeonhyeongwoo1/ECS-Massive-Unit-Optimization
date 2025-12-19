using System;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_EvolutionItem : MonoBehaviour
    {
        public EvolutionOrderType EvolutionOrderType => _evolutionOrderOrderType;
        public EvolutionType EvolutionType => _evolutionType;
        
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private EvolutionOrderType _evolutionOrderOrderType;
        [SerializeField] private EvolutionType _evolutionType;
        [SerializeField] private TextMeshProUGUI _toolTipEvolutionTypeNameText;
        [SerializeField] private TextMeshProUGUI _toolTipEvolutionValueText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _lockObject;
        [SerializeField] private GameObject _outlineObject;
        [SerializeField] private GameObject _tooltipObject;
        [SerializeField] private TextMeshProUGUI _evolutionTitleText;

        private bool _isLock;
        public void AddEvent(Action<UI_EvolutionItem> onShowToggleAction)
        {
            _button.SafeAddButtonListener(() =>
            {
                if (_isLock)
                {
                    return;
                }
                
                onShowToggleAction.Invoke(this);
            });
        }
        
        public void UpdateUI(string levelText, string toolTipEvolutionValue, string toolTipEvolutionTypeName, bool isLock)
        {
            _isLock = isLock;
            _levelText.text = $"Lv.{levelText}";
            // _toolTipEvolutionTypeNameText.text = toolTipEvolutionTypeName;
            _toolTipEvolutionValueText.text = toolTipEvolutionValue;
            _lockObject.SetActive(isLock);
            _evolutionTitleText.gameObject.SetActive(!isLock);
            transform.localScale = Vector3.one;
        }

        public void ActiveTooltip(bool isActive)
        {
            _tooltipObject.SetActive(isActive);
        }

        public void ShowHighlight(bool isActive)
        {
            _outlineObject.SetActive(isActive);   
        }

        public void ScaleUp(bool isActive)
        {
            if (isActive)
            {
                transform.DOScale(Vector3.one * 1.3f, 0.3f);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
    }
}
