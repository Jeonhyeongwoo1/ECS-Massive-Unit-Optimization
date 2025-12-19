using System;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_GachaElement : UI_ShopElement
    {
        public ShopIdType ShopIdType => _shopIdType;
        
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _remainTimeValueText;
        [SerializeField] private GameObject _remainTimeValueObject;

        [SerializeField] private Button _adsButton;
        [SerializeField] private Button _keyValueButton;
        [SerializeField] private TextMeshProUGUI _keyValueText;
        [SerializeField] private Image _keyImage;
        [SerializeField] private ShopIdType _shopIdType;
        [SerializeField] private Button _gachaProbabilityButton;

        public void AddEvent(Action<ShopIdType> onPurchaseItemAction, Action<GachaProbabilityTableType> gachaProbabilityAction)
        {
            _keyValueButton?.SafeAddButtonListener(()=>onPurchaseItemAction.Invoke(_shopIdType));
            if (_shopIdType == ShopIdType.Shop_NormalGacha)
            {
                _adsButton?.SafeAddButtonListener(() => onPurchaseItemAction.Invoke(ShopIdType.Shop_NormalGacha_Ads));
            }

            GachaProbabilityTableType gachaProbabilityTableType = GachaProbabilityTableType.Scroll;
            if (_shopIdType == ShopIdType.Shop_NormalGacha || _shopIdType == ShopIdType.Shop_NormalGacha_Ads)
            {
                gachaProbabilityTableType = GachaProbabilityTableType.EquipmentNormal;
            }
            else if(_shopIdType == ShopIdType.Shop_RareGacha)
            {
                gachaProbabilityTableType = GachaProbabilityTableType.EquipmentRare;
            }
            
            _gachaProbabilityButton.SafeAddButtonListener(()=>gachaProbabilityAction.Invoke(gachaProbabilityTableType));
        }

        public void UpdateUI(ShopData shopData, string currentKeyValue, Sprite keySprite, bool isShowAds,
            string remainTime, bool isPossiblePurchaseItem)
        {
            DataManager dataManager = Manager.I.Data;
            LocalizationData localizationData = dataManager.LocalizationDataDict[shopData.Name];
            string title = localizationData.GetValueByLanguage();
            _titleText.text = title;
            _keyValueText.text = $"{currentKeyValue}";
            _keyImage.sprite = keySprite;

            if (_remainTimeValueObject != null)
            {
                _remainTimeValueText.text = remainTime;
                _remainTimeValueObject.SetActive(!isShowAds);
            }

            if (_adsButton != null)
            {
                _adsButton.gameObject.SetActive(isShowAds);
            }

            _keyValueButton.interactable = isPossiblePurchaseItem;
            _keyValueText.color = isPossiblePurchaseItem ? Color.white : Color.red;
        }
    }
}