using System;
using MewVivor.Common;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_ShopPurchasePopup : BasePopup
    {
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TextMeshProUGUI _purchasePriceText;

        [Header("[Scroll]")]
        [SerializeField] private GameObject _scrollContentPanel;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _plusButton;
        [SerializeField] private TextMeshProUGUI _scollAmountText;

        [Header("[Gold]")] 
        [SerializeField] private GameObject _goldContentPanel;
        [SerializeField] private TextMeshProUGUI _goldPriceText;

        public void AddEvent(Action onPurchaseAction, Action onCloseAction, Action<bool> onUpdateScrollAmountAction)
        {
            _purchaseButton.SafeAddButtonListener(onPurchaseAction.Invoke);
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _minusButton.SafeAddButtonListener(() => onUpdateScrollAmountAction.Invoke(false));
            _plusButton.SafeAddButtonListener(() => onUpdateScrollAmountAction.Invoke(true));
        }

        public void UpdateUI(ShopPurchaseType shopPurchaseType, string targetPurchaseTypeAmount, string purchasePrice, bool isPossiblePurchase)
        {
            if (shopPurchaseType == ShopPurchaseType.Scroll)
            {
                _scrollContentPanel.SetActive(true);
                _goldContentPanel.SetActive(false);
                _scollAmountText.text = targetPurchaseTypeAmount;
                _purchasePriceText.text = purchasePrice;
            }
            else
            {
                _scrollContentPanel.SetActive(false);
                _goldContentPanel.SetActive(true);
                _goldPriceText.text = $"x{targetPurchaseTypeAmount}";
                _purchasePriceText.text = purchasePrice;
            }
            
            _purchaseButton.interactable = isPossiblePurchase;
            _purchasePriceText.color = isPossiblePurchase ? Color.white : Color.red;
        }
    }
}