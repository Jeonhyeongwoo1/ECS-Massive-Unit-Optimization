using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Util;
using UnityEngine;
using ShopData = MewVivor.Data.ShopData;

namespace MewVivor.Presenter
{
    public class ShopPopupPresenter : BasePresenter
    {
        private UI_ShopPopup _popup;
        private ShopModel ShopModel => ModelFactory.CreateOrGetModel<ShopModel>();
        
        public void OpenPopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_ShopPopup>();
            _popup.AddEvent((v) => OnPurchaseItemAsync(v, true).Forget(), OnOpenGachaProbabilityTablePopup);
      
            Refresh();
            SetVerticalNormalizedPosition(1);
        }

        private void OnOpenGachaProbabilityTablePopup(GachaProbabilityTableType tableType)
        {
            var gachaProbabilityTablePresenter = PresenterFactory.CreateOrGet<GachaProbabilityTablePresenter>();
            gachaProbabilityTablePresenter.OpenPopup(tableType);
        }

        public void Refresh()
        {
            if (_popup == null)
            {
                return;
            }
            
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            Dictionary<string, ShopData> shopDataDict = Manager.I.Data.ShopDataDict;
            _popup.UpdateUI(shopDataDict, userModel, ShopModel.ShopItemInfoDataList);
        }

        public async UniTask GetShopDataAsync()
        {
            var response =
                await Manager.I.Web.SendRequest<ShopResponseData>("/shop/items", null, MethodType.GET.ToString());
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Error {response.statusCode} / {response.message}");
                return;
            }
            
            List<ShopItemInfoData> shopItems = response.data.items;
            ShopModel.SetShopItemInfoData(shopItems);
        }

        public async UniTask OnPurchaseItemAsync(ShopIdType shopIdType, bool isShopPurchaseSuccessPopup)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            Debug.Log($"shop id: {shopIdType}");
            switch (shopIdType)
            {
                //ADs
                case ShopIdType.Shop_NormalGacha_Ads:
                case ShopIdType.Shop_Jewel2:
                case ShopIdType.Shop_Gold2:
                    Manager.I.Ads.ShowRewardAd(
                        () =>
                        {
                            ShopPurchaseProcess(shopIdType, ShopIAPIdType.None, isShopPurchaseSuccessPopup).Forget();
                        },
                        OnFailedShowAds);
                    return;
                case ShopIdType.Shop_Gold3:
                case ShopIdType.Shop_Gold4:
                    //Open Popup
                    ShopPurchasePopupPresenter purchasePopupPresenter =
                        PresenterFactory.CreateOrGet<ShopPurchasePopupPresenter>();
                    purchasePopupPresenter.OpenPopup(ShopPurchaseType.Gold, shopIdType.ToString());
                    return;
                case ShopIdType.Shop_RandomScroll:
                case ShopIdType.Shop_WeaponScroll:
                    purchasePopupPresenter =
                        PresenterFactory.CreateOrGet<ShopPurchasePopupPresenter>();
                    purchasePopupPresenter.OpenPopup(ShopPurchaseType.Scroll, shopIdType.ToString());
                    return;
            }

            ShopData shopData = Manager.I.Data.ShopDataDict[shopIdType.ToString()];
            ShopIAPIdType shopIAPIdType = shopData.StoreProductId;
            try
            {
                await ShopPurchaseProcess(shopIdType, shopIAPIdType, isShopPurchaseSuccessPopup);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed error {e.Message}");   
            }

            if (_popup != null)
            {
                UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
                Dictionary<string, ShopData> shopDataDict = Manager.I.Data.ShopDataDict;
                _popup.UpdateUI(shopDataDict, userModel, ShopModel.ShopItemInfoDataList);
            }
        }

        private void OnFailedShowAds()
        {
            string message = Manager.I.Data.LocalizationDataDict["Ads_Not_Ready"].GetValueByLanguage();
            Manager.I.UI.OpenSystemPopup(message);
        }

        private async UniTask RequestShopPurchasedItem(ShopIdType shopIdType, ShopIAPIdType shopIAPIdType,
            bool isShopPurchaseSuccessPopup, ReceiptData? receiptData)
        {
            TaskHelper.InitTcs();
            ShopPurchaseRequestData shopPurchaseRequestData = null;
            if (receiptData.HasValue)
            {
                ReceiptData data = receiptData.Value;
                shopPurchaseRequestData = new ShopPurchaseRequestData
                {
                    os = data.os,
                    bundleId = data.bundleId,
                    productId = data.productId,
                    transactionId = data.transactionId,
                    quantity = 1
                };
            }
            else
            {
                shopPurchaseRequestData = new ShopPurchaseRequestData()
                {
                    quantity = 1
                };
            }

            var response =
                await Manager.I.Web.SendRequest<ShopResponseData>($"/shop/items/{shopIdType}", shopPurchaseRequestData,
                    MethodType.POST.ToString());

            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Error {response.statusCode} / {response.message}");
                return;
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Gold.Value;
            int jewel = response.data.inventory.jewel - userModel.Jewel.Value;
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
            
            userModel.SetInventory(response.data.inventory);
            List<ShopItemInfoData> shopItems = response.data.items;
            if (_popup != null)
            {
                Dictionary<string, ShopData> shopDataDict = Manager.I.Data.ShopDataDict;
                _popup.UpdateUI(shopDataDict, userModel, shopItems);
            }

            List<Equipment> grantedEquipments = null;
            if (response.data.grantedEquipments != null && response.data.grantedEquipments.Count > 0)
            {
                grantedEquipments = new List<Equipment>();
                foreach (UserEquipmentData data in response.data.grantedEquipments)
                {
                    Equipment equipment = userModel.AddEquipment(data);
                    grantedEquipments.Add(equipment);
                }
            }

            ShopModel.SetShopItemInfoData(shopItems);

            if (isShopPurchaseSuccessPopup)
            {
                switch (shopIdType)
                {
                    case ShopIdType.Shop_NormalGacha_Ads:
                    case ShopIdType.Shop_NormalGacha:
                    case ShopIdType.Shop_RareGacha:
                        EquipmentBoxOpenPopupPresenter equipmentBoxOpenPopupPresenter =
                            PresenterFactory.CreateOrGet<EquipmentBoxOpenPopupPresenter>();
                        equipmentBoxOpenPopupPresenter.OpenPopup(grantedEquipments);
                        break;
                    default:
                        ShopPurchaseSuccessPopupPresenter shopPurchaseSuccessPopupPresenter =
                            PresenterFactory.CreateOrGet<ShopPurchaseSuccessPopupPresenter>();
                        shopPurchaseSuccessPopupPresenter.OpenPopup(response.data.grantedItems);
                        break;
                }
            }
        }

        private void OnFailedPurchasedItem()
        {
            string message = Manager.I.Data.LocalizationDataDict["Shop_FailPurchase"].GetValueByLanguage();
            Manager.I.UI.OpenSystemPopup(message);
        }

        private async UniTask ShopPurchaseProcess(ShopIdType shopIdType, ShopIAPIdType shopIAPIdType, bool isShopPurchaseSuccessPopup)
        {
            if (shopIAPIdType == ShopIAPIdType.None)
            {
                await RequestShopPurchasedItem(shopIdType, shopIAPIdType, isShopPurchaseSuccessPopup, null);
            }
            else
            {
                Manager.I.IAP.BuyProduct(shopIAPIdType,
                    (receiptData) => RequestShopPurchasedItem(shopIdType, shopIAPIdType, isShopPurchaseSuccessPopup, receiptData)
                        .Forget(),
                    OnFailedPurchasedItem, false);
            }
        }

        public void ClosePopup() 
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        public void SetVerticalNormalizedPosition(float ratio)
        {
            _popup.SetVerticalNormalizedPosition(ratio);
        }
    }
}