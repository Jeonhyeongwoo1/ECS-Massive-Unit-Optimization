using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.UISubItemElement;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_VipShopPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private UI_VipSubElement _vipSubElement;
        [SerializeField] private UI_VipSubElement _vvipSubElement;

        public void AddEvent(Action onClosePopupAction, Action<string> onPurchaseItemAction)
        {
            _closeButton.SafeAddButtonListener(onClosePopupAction.Invoke);
            _vipSubElement.AddEvent(onPurchaseItemAction.Invoke);
            _vvipSubElement.AddEvent(onPurchaseItemAction.Invoke);
        }
        
        public void UpdateUI(List<ShopItemInfoData> shopItemInfoDataList, Dictionary<string, Data.ShopData> shopDataDict)
        {
            {
                ShopItemInfoData vipItemData =
                    shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_VIP.ToString());
                Data.ShopData vipShopData = shopDataDict[vipItemData.itemId];

                string vipEfficient = $"{vipShopData.Efficient * 100}";
                string price = $"{Manager.I.IAP.GetProductPriceString(ShopIAPIdType.shop_vip)}";
                Sprite vipRewardItemSprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                ShopSubscribeData subscribeData = Manager.I.Data.ShopSubscribeDataDict[VipType.Shop_VIP.ToString()];
                VipRewardData rewardData = new VipRewardData
                {
                    rewardSprite = vipRewardItemSprite,
                    amount = vipShopData.Reward_Amount[0]
                };

                VipRewardData dailyRewardData = new VipRewardData
                {
                    rewardSprite = vipRewardItemSprite,
                    amount = subscribeData.Reward_Amount
                };

                List<VipRewardData> list = new List<VipRewardData>
                {
                    rewardData,
                    dailyRewardData
                };

                UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
                ShopSubscriptionInfoData vipData = userModel.SubscriptionInfo["Shop_VIP"];
                string remainTime = "";
                if (vipData.endDate.HasValue)
                {
                    TimeSpan timeSpan = vipData.endDate.Value - DateTime.UtcNow;
                    int day = timeSpan.Days;
                    int hours = timeSpan.Hours;
                    int minutes = timeSpan.Minutes;
                    if (day >= 1)
                    {
                        remainTime = $"{day}d{hours}h{minutes}m";
                    }
                    else
                    {
                        remainTime = $"{hours}h{minutes}m";
                    }
                }
                _vipSubElement.UpdateUI(vipItemData.itemId, vipEfficient, price, list, vipItemData.isPurchasable,
                    remainTime);
            }

            {
                ShopItemInfoData vipItemData =
                    shopItemInfoDataList.Find(v => v.itemId == ShopIdType.Shop_VVIP.ToString());
                Data.ShopData vipShopData = shopDataDict[vipItemData.itemId];
                string vipEfficient = $"{vipShopData.Efficient * 100}";
                string price = $"{Manager.I.IAP.GetProductPriceString(ShopIAPIdType.shop_vvip)}";
                Sprite vipRewardItemSprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                ShopSubscribeData subscribeData = Manager.I.Data.ShopSubscribeDataDict[VipType.Shop_VVIP.ToString()];
                VipRewardData rewardData = new VipRewardData
                {
                    rewardSprite = vipRewardItemSprite,
                    amount = vipShopData.Reward_Amount[0]
                };

                VipRewardData dailyRewardData = new VipRewardData
                {
                    rewardSprite = vipRewardItemSprite,
                    amount = subscribeData.Reward_Amount
                };

                List<VipRewardData> list = new List<VipRewardData>
                {
                    rewardData,
                    dailyRewardData
                };
                
                UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
                ShopSubscriptionInfoData vipData = userModel.SubscriptionInfo["Shop_VVIP"];
                string remainTime = "";
                if (vipData.endDate.HasValue)
                {
                    TimeSpan timeSpan = vipData.endDate.Value - DateTime.UtcNow;
                    int hours = timeSpan.Hours;
                    int minutes = timeSpan.Minutes;
                    remainTime = $"{hours}h{minutes}m";
                }
                _vvipSubElement.UpdateUI(vipItemData.itemId, vipEfficient, price, list, vipItemData.isPurchasable,
                    remainTime);
            }
        }
    }
}