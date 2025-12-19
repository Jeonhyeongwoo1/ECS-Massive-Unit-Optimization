using System;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_GoldElement : UI_ShopElement
    {
        public ShopIdType ShopIdType => _shopIdType;

        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _costValueText;
        [SerializeField] private Image _goodsImage;
        [SerializeField] private ShopIdType _shopIdType;
        [SerializeField] private Button _purhcaseButton;
        [SerializeField] private GameObject _purchasedCompletedObject;
        [SerializeField] private GameObject _redot;

        public void AddEvent(Action<ShopIdType> onPurchaseItemAction, bool isFree)
        {
            if (_shopIdType == ShopIdType.Shop_Gold1)
            {
                _purhcaseButton.SafeAddButtonListener(() =>
                    onPurchaseItemAction.Invoke(isFree ? ShopIdType.Shop_Gold1 : ShopIdType.Shop_Gold2));
            }
            else
            {
                _purhcaseButton.SafeAddButtonListener(()=>onPurchaseItemAction.Invoke(_shopIdType));
            }
        }
        
        public void UpdateUI(ShopData shopData, Sprite goodsSprite, string costValue, bool isCompletedPurchase, bool isPossiblePurchaseItem)
        {
            if (goodsSprite == null)
            {
                _goodsImage.gameObject.SetActive(false);
                _costValueText.transform.localPosition = Vector3.zero;
            }
            else
            {
                _goodsImage.sprite = goodsSprite;
                _goodsImage.gameObject.SetActive(true);
                _costValueText.transform.localPosition = new Vector3(25, 0);
            }
            
            _goldText.text = shopData.Reward_Amount[0].ToString();
            //무료 -> 광고 2회
            _costValueText.text = costValue;
            if (_purchasedCompletedObject != null)
            {
                _purchasedCompletedObject.SetActive(isCompletedPurchase);
            }
            
            if (_redot != null)
            {
                _redot.SetActive(!isCompletedPurchase);
            }
            _purhcaseButton.interactable = isPossiblePurchaseItem;
            _costValueText.color = isPossiblePurchaseItem ? Color.white : Color.red;
        }
    }
}