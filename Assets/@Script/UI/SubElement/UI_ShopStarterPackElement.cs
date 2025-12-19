using System;
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
    public class UI_ShopStarterPackElement : UI_ShopElement
    {
        [SerializeField] private TextMeshProUGUI _efficientValueText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private UI_ShopEquipItem _uiEquipItem;
        [SerializeField] private UI_RewardItem _uiRewardItem1;
        [SerializeField] private UI_RewardItem _uiRewardItem2;
        [SerializeField] private Button _purchaseButton;

        public void AddEvent(Action<ShopIdType> onPurchaseItemAction)
        {
            _purchaseButton.SafeAddButtonListener(() =>
                onPurchaseItemAction.Invoke(ShopIdType.Shop_PackageNewbie));
        }
        
        public void UpdateUI(ShopData shopData, ShopItemInfoData shopItemInfoData)
        {
            if (!shopItemInfoData.isPurchasable)
            {
                gameObject.SetActive(false);
            }

            _efficientValueText.text = $"x{shopData.Efficient * 100}";
            //IAP
            _priceText.text = Manager.I.IAP.GetProductPriceString(ShopIAPIdType.shop_pack_1);

            ResourcesManager resourcesManager = Manager.I.Resource;
            DataManager dataManager = Manager.I.Data;
            EquipmentData equipmentData = dataManager.EquipmentDataDict[shopData.Reward_ID[0]];
            Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentData.EquipmentType}_Icon.sprite");
            Sprite gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipmentData.Grade);
            _uiEquipItem.UpdateUI(equipSprite, equipTypeSprite, gradeSprite);

            ItemData rewardItemData1 = dataManager.ItemDataDict[shopData.Reward_ID[1]];
            Sprite rewardItemSprite1 = resourcesManager.Load<Sprite>(rewardItemData1.SpriteName);
            _uiRewardItem1.UpdateUI(rewardItemSprite1, shopData.Reward_Amount[1]);
            
            ItemData rewardItemData2 = dataManager.ItemDataDict[shopData.Reward_ID[2]];
            Sprite rewardItemSprite2 = resourcesManager.Load<Sprite>(rewardItemData2.SpriteName);
            _uiRewardItem2.UpdateUI(rewardItemSprite2, shopData.Reward_Amount[2]);
        }

    }
}