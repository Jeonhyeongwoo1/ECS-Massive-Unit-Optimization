using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ShopData = MewVivor.Data.ShopData;

namespace MewVivor.UISubItemElement
{
    public class UI_ShopPackageElement : UI_ShopElement
    {
        [SerializeField] private TextMeshProUGUI _efficientValueText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private List<UI_RewardItem> _rewardItemList;
        [SerializeField] private Button _purchaseButton;

        public void AddEvent(Action<ShopIdType> onPurchaseItemAction, ShopIdType shopIdType)
        {
            _purchaseButton.SafeAddButtonListener(()=>onPurchaseItemAction.Invoke(shopIdType));
        }
        
        public void UpdateUI(ShopData shopData, ShopItemInfoData shopItemInfoData, ShopIdType shopIdType)
        {
            if (!shopItemInfoData.isPurchasable)
            {
                gameObject.SetActive(false);
                return;
            }

            _priceText.text =
                $"{Manager.I.IAP.GetProductPriceString(shopIdType == ShopIdType.Shop_PackageGrowth ? ShopIAPIdType.shop_pack_2 : ShopIAPIdType.shop_pack_3)}";
            _efficientValueText.text = $"x{shopData.Efficient * 100}";

            DataManager dataManager = Manager.I.Data;
            ResourcesManager resourcesManager = Manager.I.Resource;
            
            for (var i = 0; i < shopData.Reward_ID.Count; i++)
            {
                ItemData itemData = dataManager.ItemDataDict[shopData.Reward_ID[i]];
                Sprite rewardItemSprite = resourcesManager.Load<Sprite>(itemData.SpriteName);
                _rewardItemList[i].UpdateUI(rewardItemSprite, shopData.Reward_Amount[i]);
            }
        }
    }
}