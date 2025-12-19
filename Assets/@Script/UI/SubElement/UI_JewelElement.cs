using System;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_JewelElement : UI_ShopElement
    {
        public ShopIdType ShopIdType => _shopIdType;

        [SerializeField] private TextMeshProUGUI _jewelText;
        [SerializeField] private TextMeshProUGUI _costValueText;
        [SerializeField] private Button _getButton;
        [SerializeField] private GameObject _reddotObject;
        [SerializeField] private GameObject _firstPurchaseDoublePanelObject;
        [SerializeField] private Image _goodsImage;
        [SerializeField] private ShopIdType _shopIdType;
        [SerializeField] private GameObject _purchasedCompletedObject;
        [SerializeField] private GameObject _redot;
        
        public void AddEvent(Action<ShopIdType> onPurchaseItemAction, bool isFree)
        {
            if (_shopIdType == ShopIdType.Shop_Jewel1)
            {
                _getButton.SafeAddButtonListener(() =>
                {
                    ShopIdType shopIdType = isFree ? ShopIdType.Shop_Jewel1 : ShopIdType.Shop_Jewel2;
                    onPurchaseItemAction.Invoke(shopIdType);
                });
            }
            else
            {
                _getButton.SafeAddButtonListener(()=>onPurchaseItemAction.Invoke(_shopIdType));
            }
        }
                
        public void UpdateUI(ShopData shopData, Sprite goodsSprite, string costValue, bool isCompletedPurchase, bool isDouble)
        {
            _goodsImage.gameObject.SetActive(goodsSprite != null);
            _goodsImage.sprite = goodsSprite;
            _jewelText.text = shopData.Reward_Amount[0].ToString();
            _costValueText.text = costValue;
            
            if (_firstPurchaseDoublePanelObject)
            {
                _firstPurchaseDoublePanelObject.SetActive(isDouble);
            }
            
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
        }
    }
}