using System;
using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.UISubItemElement;
using UnityEngine;
using UnityEngine.UI;
using ShopData = MewVivor.Data.ShopData;

namespace MewVivor.Popup
{
    public class UI_ShopPopup : BasePopup
    {
        [SerializeField] private UI_ShopStarterPackElement _uiShopStarterPackElement;
        [SerializeField] private UI_ShopPackageElement _growthPackageElement;
        [SerializeField] private UI_ShopPackageElement _costPackageElement;
        [SerializeField] private List<UI_GachaElement> _gachaElementList;
        [SerializeField] private List<UI_GoldElement> _goldElementList;
        [SerializeField] private List<UI_JewelElement> _jewelElementList;
        [SerializeField] private UI_InfiniteTicketShopElement _infiniteTicket;
        [SerializeField] private List<UI_ScrollShopElement> _scrollShopElementList;
        [SerializeField] private ScrollRect _scrollRect;

        private Action<ShopIdType> _onPurchaseItemAction;
        private Action<GachaProbabilityTableType> _gachaProbabilityAction;
        
        public void AddEvent(Action<ShopIdType> onPurchaseItemAction, Action<GachaProbabilityTableType> gachaProbabilityAction)
        {
            _onPurchaseItemAction = onPurchaseItemAction;
            _gachaProbabilityAction = gachaProbabilityAction;
        }

        public void UpdateUI(Dictionary<string, ShopData> shopDataDict, UserModel userModel,
            List<ShopItemInfoData> shopItemList)
        {
            ShopData starterPackData = shopDataDict[ShopIdType.Shop_PackageNewbie.ToString()];
            ShopItemInfoData shopItemInfoData =
                shopItemList.Find(v => v.itemId == ShopIdType.Shop_PackageNewbie.ToString());
            _uiShopStarterPackElement.AddEvent(_onPurchaseItemAction);
            _uiShopStarterPackElement.UpdateUI(starterPackData, shopItemInfoData);

            ShopData growthPackageData = shopDataDict[ShopIdType.Shop_PackageGrowth.ToString()];
            ShopItemInfoData shopPackageGrowthInfoData =
                shopItemList.Find(v => v.itemId == ShopIdType.Shop_PackageGrowth.ToString());
            _growthPackageElement.AddEvent(_onPurchaseItemAction, ShopIdType.Shop_PackageGrowth);
            _growthPackageElement.UpdateUI(growthPackageData, shopPackageGrowthInfoData, ShopIdType.Shop_PackageGrowth);

            ShopData costPackageData = shopDataDict[ShopIdType.Shop_PackageCost.ToString()];
            ShopItemInfoData shopPackageCostInfoData =
                shopItemList.Find(v => v.itemId == ShopIdType.Shop_PackageCost.ToString());
            _costPackageElement.AddEvent(_onPurchaseItemAction, ShopIdType.Shop_PackageCost);
            _costPackageElement.UpdateUI(costPackageData, shopPackageCostInfoData, ShopIdType.Shop_PackageCost);

            UpdateGachaElement(shopDataDict, userModel, shopItemList);
            UpdateGoldElement(shopDataDict, userModel, shopItemList);
            UpdateJewelElement(shopDataDict, userModel, shopItemList);
            UpdateInfiniteTicket(shopDataDict, shopItemList, userModel);
            UpdateScrollElement(shopDataDict, shopItemList, userModel);
        }

        public void SetVerticalNormalizedPosition(float ratio)
        {
            _scrollRect.verticalNormalizedPosition = ratio;
        }

        private void UpdateScrollElement(Dictionary<string, ShopData> shopDataDict,
            List<ShopItemInfoData> shopItemInfoList, UserModel userModel)
        {
            foreach (UI_ScrollShopElement uiScrollShopElement in _scrollShopElementList)
            {
                ShopData shopData = shopDataDict[uiScrollShopElement.ShopIdType.ToString()];
                uiScrollShopElement.AddEvent(_onPurchaseItemAction, _gachaProbabilityAction);
                bool isPossiblePurchase = shopData.CostPrice[0] <= userModel.Jewel.Value;
                uiScrollShopElement.UpdateUI(shopData.CostPrice[0].ToString(), isPossiblePurchase);
            }
        }

        private void UpdateInfiniteTicket(Dictionary<string, ShopData> shopDataDict,
            List<ShopItemInfoData> shopItemInfoList, UserModel userModel)
        {
            ShopItemInfoData shopItemInfoData =
                shopItemInfoList.Find(v => v.itemId == ShopIdType.Shop_InfiniteTicket1.ToString());
            ShopData infiniteTicketData = shopDataDict[ShopIdType.Shop_InfiniteTicket1.ToString()];
            string remainPurchaseCount =
                $"({infiniteTicketData.BuyLimitCount - shopItemInfoData.buyCount}/{infiniteTicketData.BuyLimitCount})";
            bool isPossiblePurchase = shopItemInfoData.isPurchasable;
            bool isCompletedAllPurchased = infiniteTicketData.BuyLimitCount == shopItemInfoData.buyCount;
            _infiniteTicket.AddEvent(_onPurchaseItemAction);
            _infiniteTicket.UpdateUI(infiniteTicketData.CostPrice[0].ToString(), remainPurchaseCount,
                isPossiblePurchase, isCompletedAllPurchased);
        }

        private void UpdateGoldElement(Dictionary<string, ShopData> shopDataDict, UserModel userModel, List<ShopItemInfoData> shopItemInfoList)
        {
            foreach (UI_GoldElement uiGoldElement in _goldElementList)
            {
                ShopIdType shopIdType = uiGoldElement.ShopIdType;
                ShopData gachaData = shopDataDict[shopIdType.ToString()];
                ShopItemInfoData shopItemInfoData = shopItemInfoList.Find(v => v.itemId == shopIdType.ToString());
                if (shopItemInfoData.itemId == ShopIdType.Shop_Gold1.ToString())
                {
                    bool isFree = shopItemInfoData.buyCount == 0;
                    if (!isFree)
                    {
                        shopItemInfoData = shopItemInfoList.Find(v => v.itemId == ShopIdType.Shop_Gold2.ToString());
                        gachaData = shopDataDict[ShopIdType.Shop_Gold2.ToString()];
                    }
                    
                    string costString = Manager.I.Data.LocalizationDataDict["Pop_HuntPass_Free"].GetValueByLanguage();
                    bool isCompletedPurchase = shopItemInfoData.buyCount >= gachaData.BuyLimitCount;
                    var iconSprite = isFree ? null : Manager.I.Resource.Load<Sprite>(Const.AdsSpriteName);
                    uiGoldElement.AddEvent(_onPurchaseItemAction, isFree);
                    uiGoldElement.UpdateUI(gachaData, iconSprite, costString, isCompletedPurchase, true);
                }
                else
                {
                    bool isPossiblePurchaseItem = userModel.Jewel.Value > gachaData.CostPrice[0];
                    string costString = gachaData.CostPrice[0].ToString();
                    var iconSprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                    uiGoldElement.AddEvent(_onPurchaseItemAction, false);
                    uiGoldElement.UpdateUI(gachaData, iconSprite, costString, false, isPossiblePurchaseItem);
                }
            }
        }
        
        private void UpdateJewelElement(Dictionary<string, ShopData> shopDataDict, UserModel userModel, List<ShopItemInfoData> shopItemInfoList)
        {
            foreach (UI_JewelElement uiJewelElement in _jewelElementList)
            {
                ShopIdType shopIdType = uiJewelElement.ShopIdType;
                ShopData gachaData = shopDataDict[shopIdType.ToString()];
                ShopItemInfoData shopItemInfoData = shopItemInfoList.Find(v => v.itemId == shopIdType.ToString());
                if (shopItemInfoData.itemId == ShopIdType.Shop_Jewel1.ToString())
                {
                    bool isFree = shopItemInfoData.buyCount == 0;
                    if (!isFree)
                    {
                        shopItemInfoData = shopItemInfoList.Find(v => v.itemId == ShopIdType.Shop_Jewel2.ToString());
                        gachaData = shopDataDict[ShopIdType.Shop_Jewel2.ToString()];
                    }

                    string costString = Manager.I.Data.LocalizationDataDict["Pop_HuntPass_Free"].GetValueByLanguage();
                    bool isCompletedPurchase = shopItemInfoData.buyCount >= gachaData.BuyLimitCount;
                    var iconSprite = isFree ? null : Manager.I.Resource.Load<Sprite>(Const.AdsSpriteName);
                    uiJewelElement.AddEvent(_onPurchaseItemAction, isFree);
                    uiJewelElement.UpdateUI(gachaData, iconSprite, costString, isCompletedPurchase, false);
                }
                else
                {
                    string costString = $"{Manager.I.IAP.GetProductPriceString(gachaData.StoreProductId)}";
                    bool isAvailableDoubleValue = shopItemInfoData.buyCount < 1;
                    // var iconSprite = Manager.I.Resource.Load<Sprite>(Const.JewelSprite);
                    uiJewelElement.AddEvent(_onPurchaseItemAction, false);
                    uiJewelElement.UpdateUI(gachaData, null, costString, false, isAvailableDoubleValue);
                }
            }
        }

        private void UpdateGachaElement(Dictionary<string, ShopData> shopDataDict, UserModel userModel,
            List<ShopItemInfoData> shopItemList)
        {
            foreach (UI_GachaElement uiGachaElement in _gachaElementList)
            {
                ShopData gachaData = shopDataDict[uiGachaElement.ShopIdType.ToString()];
                string keyValue = "";
                Sprite keySprite = null;
                bool isShowAds = false;
                bool isPossiblePurchaseItem = false;
                if (uiGachaElement.ShopIdType == ShopIdType.Shop_NormalGacha)
                {
                    ShopItemInfoData shopItemInfoData =
                        shopItemList.Find(v => v.itemId == ShopIdType.Shop_NormalGacha_Ads.ToString());
                    isShowAds = shopItemInfoData.buyCount <= gachaData.BuyLimitCount;
                    int normalKey = userModel.Inventory.silverKey;
                    if (normalKey == 0)
                    {
                        isPossiblePurchaseItem = userModel.Jewel.Value >= gachaData.CostPrice[1];
                        keyValue = $"{gachaData.CostPrice[1]}";
                        keySprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                    }
                    else
                    {
                        isPossiblePurchaseItem = userModel.Inventory.silverKey >= gachaData.CostPrice[0];
                        keyValue = $"{userModel.Inventory.silverKey}/{gachaData.CostPrice[0]}";
                        keySprite = Manager.I.Resource.Load<Sprite>("Key_Silver_Icon");
                    }
                }
                else if (uiGachaElement.ShopIdType == ShopIdType.Shop_RareGacha)
                {
                    int goldenKey = userModel.Inventory.goldKey;
                    if (goldenKey == 0)
                    {
                        isPossiblePurchaseItem = userModel.Jewel.Value >= gachaData.CostPrice[1];
                        keyValue = $"{gachaData.CostPrice[1]}";
                        keySprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                    }
                    else
                    {
                        isPossiblePurchaseItem = userModel.Inventory.goldKey >= gachaData.CostPrice[0];
                        keyValue = $"{userModel.Inventory.goldKey}/{gachaData.CostPrice[0]}";
                        keySprite = Manager.I.Resource.Load<Sprite>("Key_Gold_Icon");
                    }
                }
                
                DateTime utcNow = DateTime.UtcNow;
                DateTime nextUtcMidnight = utcNow.Date.AddDays(1);
                TimeSpan timeUntilReset = nextUtcMidnight - utcNow;
                string remainTime = $"{timeUntilReset.Hours}h{timeUntilReset.Minutes}m";
                uiGachaElement.AddEvent(_onPurchaseItemAction, _gachaProbabilityAction);
                uiGachaElement.UpdateUI(gachaData, keyValue, keySprite, isShowAds, remainTime, isPossiblePurchaseItem);
            }
        }
    }
}