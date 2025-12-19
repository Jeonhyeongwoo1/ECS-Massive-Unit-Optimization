using MewVivor.Common;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_GachaProbabilityTablePopup : BasePopup
    {
        public Transform GachaElementParent => _scrollRect.content;
        
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private TextMeshProUGUI _titleText;

        public void Release()
        {
            var childs = Utils.GetChildComponent<UI_GachaProbabilityTableElement>(_scrollRect.content);
            if (childs != null)
            {
                foreach (UI_GachaProbabilityTableElement uiGachaProbabilityTableElement in childs)
                {
                    Manager.I.Pool.ReleaseObject(nameof(UI_GachaProbabilityTableElement), uiGachaProbabilityTableElement.gameObject);
                }
            }
        }
        
        public void UpdateUI(string title)
        {
            _titleText.text = title;
            
        }
    }
}