using System;
using MewVivor.Common;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_ScrollShopElement : UI_ShopElement
    {
        public ShopIdType ShopIdType => _shopIdType;

        [SerializeField] private TextMeshProUGUI _costAmounText;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private ShopIdType _shopIdType;
        [SerializeField] private Button _gachaProbabilityButton;
        

        public void AddEvent(Action<ShopIdType> onPurchaseAction, Action<GachaProbabilityTableType> gachaProbabilityAction)
        {
            _purchaseButton.SafeAddButtonListener(() => onPurchaseAction.Invoke(_shopIdType));
            _gachaProbabilityButton.SafeAddButtonListener(()=> gachaProbabilityAction.Invoke(GachaProbabilityTableType.Scroll));
        }

        public void UpdateUI(string costAmount, bool isPossiblePurchaseItem)
        {
            _costAmounText.text = costAmount;
            _purchaseButton.interactable = isPossiblePurchaseItem;
            _costAmounText.color = isPossiblePurchaseItem ? Color.white : Color.red;
        }
    }
}