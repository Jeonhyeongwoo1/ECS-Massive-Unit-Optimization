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
    public class ShopPurchasePopupPresenter : BasePresenter
    {
        private UI_ShopPurchasePopup _popup;
        private ShopPurchaseType _currentShopPurchaseType;
        private int _scrollAmount = 1;
        private string _shopItemID;
        private ShopModel ShopModel => ModelFactory.CreateOrGetModel<ShopModel>();
        
        public void OpenPopup(ShopPurchaseType shopPurchaseType, string shopItemID)
        {
            _scrollAmount = 1;
            _currentShopPurchaseType = shopPurchaseType;
            _shopItemID = shopItemID;
            
            _popup = Manager.I.UI.OpenPopup<UI_ShopPurchasePopup>();
            _popup.AddEvent(OnPurchaseItem, OnClosePopup, OnUpdateScrollAmount);

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int jewel = userModel.Jewel.Value;
            ShopData shopData = Manager.I.Data.ShopDataDict[shopItemID];
            int costPrice = shopData.CostPrice[0];
            int count = shopData.Reward_Amount[0];
            bool isPossiblePurchase = jewel >= costPrice;
            _popup.UpdateUI(_currentShopPurchaseType, count.ToString(), costPrice.ToString(), isPossiblePurchase);
        }

        private void OnUpdateScrollAmount(bool isPlus)
        {
            if (isPlus)
            {
                _scrollAmount++;
            }
            else
            {
                if (_scrollAmount > 1)
                {
                    _scrollAmount--;
                }
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int jewel = userModel.Jewel.Value;
            ShopData shopData = Manager.I.Data.ShopDataDict[_shopItemID];
            int costPrice = shopData.CostPrice[0] * _scrollAmount;
            bool isPossiblePurchase = jewel >= costPrice;
            _popup.UpdateUI(_currentShopPurchaseType, _scrollAmount.ToString(), costPrice.ToString(), isPossiblePurchase);
        }

        private void OnClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        private async void OnPurchaseItem()
        {
            ShopPurchaseRequestData requestData = null;
            if (_shopItemID == ShopIdType.Shop_WeaponScroll.ToString() ||
                _shopItemID == ShopIdType.Shop_RandomScroll.ToString())
            {
                requestData = new ShopPurchaseRequestData();
                requestData.quantity = _scrollAmount;
            }

            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<ShopResponseData>($"/shop/items/{_shopItemID}", requestData,
                    MethodType.POST.ToString());
            TaskHelper.CompleteTcs();
            
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Error {response.statusCode} / {response.message}");
                switch (response.message)
                {
                    case ServerMessageType.NOT_ENOUGH_COST:
                        string message = Manager.I.Data.LocalizationDataDict["NOT_Enought_Gold"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                    case ServerMessageType.NOT_ENOUGH_JEWEL:
                        message = Manager.I.Data.LocalizationDataDict["NOT_Enought_Jewel"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                }
                
                OnClosePopup();
                return;
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Gold.Value;
            int jewel = response.data.inventory.jewel - userModel.Jewel.Value;
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold).Forget();
            }

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
            ShopPopupPresenter shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
            shopPopupPresenter.Refresh();
        }
    }
}