using System;
using MewVivor.Common;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_InfiniteTicketShopElement : UI_ShopElement
    {
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TextMeshProUGUI _costAmountText;
        [SerializeField] private TextMeshProUGUI _remainPurchaseCountText;
        [SerializeField] private GameObject _completedPurchasedGameObject;
        
        public void AddEvent(Action<ShopIdType> onPurchaseAction)
        {
            _purchaseButton.SafeAddButtonListener(() => onPurchaseAction.Invoke(ShopIdType.Shop_InfiniteTicket1));
        }

        public void UpdateUI(string costAmount, string remainPurchaseCount, bool isPossiblePurchaseItem, bool isCompletedAllPurchased)
        {
            _costAmountText.text = costAmount;
            _remainPurchaseCountText.text = remainPurchaseCount;
            _purchaseButton.interactable = isPossiblePurchaseItem;
            _costAmountText.color = isPossiblePurchaseItem ? Color.white : Color.red;

            if (_completedPurchasedGameObject != null)
            {
                _completedPurchasedGameObject.SetActive(isCompletedAllPurchased);
            }
        }
    }
}