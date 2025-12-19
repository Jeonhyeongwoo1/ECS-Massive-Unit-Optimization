using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using MewVivor.Util;
using UnityEngine;
using ShopData = MewVivor.Data.ShopData;

namespace MewVivor.Presenter
{
    public class VipShopPopupPresenter : BasePresenter
    {
        private UI_VipShopPopup _popup;
        private ShopModel ShopModel => ModelFactory.CreateOrGetModel<ShopModel>();

        public void OpenPopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_VipShopPopup>();
            _popup.AddEvent(OnClosePopup, OnPurchaseItem);
            _popup.UpdateUI(ShopModel.ShopItemInfoDataList, Manager.I.Data.ShopDataDict);
        }

        private void OnPurchaseItem(string itemId)
        {
            var shopItemData = ShopModel.ShopItemInfoDataList.Find(v => v.itemId == itemId);
            if (shopItemData == null)
            {
                return;
            }

            if (!shopItemData.isPurchasable)
            {
                return;
            }

//             bool isTest = false;
// #if UNITY_EDITOR
//             isTest = true;
// #endif
            ShopData shopData = Manager.I.Data.ShopDataDict[itemId];
            Manager.I.IAP.BuyProduct(shopData.StoreProductId,
                (receiptData) => RequestShopPurchasedItem(itemId, receiptData)
                    .Forget(),
                OnFailedPurchasedItem, false);
        }

        private async UniTaskVoid RequestShopPurchasedItem(string itemId, ReceiptData? receiptData)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
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
                string os = "";
#if UNITY_EDITOR
                os = OS.EDITOR.ToString();
#elif UNITY_ANDROID
                os = OS.AOS.ToString();
#elif UNITY_IOS
                os = OS.IOS.ToString();
#endif

                shopPurchaseRequestData = new ShopPurchaseRequestData()
                {
                    os = os,
                    quantity = 1
                };
            }
            
            
            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<ShopResponseData>($"/shop/items/{itemId}", shopPurchaseRequestData,
                    MethodType.POST.ToString());
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                string message = Manager.I.Data.LocalizationDataDict["Shop_FailPurchase"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                OnClosePopup();
                return;
            }
            
            var userResponse = await Manager.I.Web.SendRequest<GetUserResponseDataData>("/user", null, MethodType.GET.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                return;
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Gold.Value;
            int jewel = response.data.inventory.jewel - userModel.Jewel.Value;
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }

            userModel.SetUserData(userResponse.data);
            userModel.SetInventory(response.data.inventory);
            // List<Equipment> grantedEquipments = null;
            // if (response.data.grantedEquipments != null && response.data.grantedEquipments.Count > 0)
            // {
            //     grantedEquipments = new List<Equipment>();
            //     foreach (UserEquipmentData data in response.data.grantedEquipments)
            //     {
            //         Equipment equipment = userModel.AddEquipment(data);
            //         grantedEquipments.Add(equipment);
            //     }
            // }

            ShopModel.SetShopItemInfoData(response.data.items);
            OnClosePopup();
            ShopPurchaseSuccessPopupPresenter shopPurchaseSuccessPopupPresenter =
                PresenterFactory.CreateOrGet<ShopPurchaseSuccessPopupPresenter>();
            shopPurchaseSuccessPopupPresenter.OpenPopup(response.data.grantedItems);
        }

        private void OnFailedPurchasedItem()
        {
            string message = Manager.I.Data.LocalizationDataDict["Shop_FailPurchase"].GetValueByLanguage();
            Manager.I.UI.OpenSystemPopup(message);
        }

        private void OnClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }
    }
}