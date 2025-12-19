using System;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Util;
using UnityEngine;
using EquipmentLevelData = MewVivor.Data.EquipmentLevelData;

namespace MewVivor.Presenter
{
    public class EquipmentInfoPopupPresenter : BasePresenter
    {
        private UI_EquipmentInfoPopup _popup;
        
        public void ShowPopup(string id)
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            Equipment equipment = userModel.FindEquippedItemOrUnEquippedItem(id);
            if (equipment == null)
            {
                return;
            }
            
            _popup = Manager.I.UI.OpenPopup<UI_EquipmentInfoPopup>();
            _popup.AddEvent(() => OnUnquip(equipment),
                () => OnLevelUp(equipment)
                , () => OnAllLevelUp(equipment)
                , () => OnEquip(equipment),
                () => OnOpenLevelDownPopup(equipment));
            _popup.UpdateUI(equipment, userModel.Gold.Value, userModel.GetScrollCount(equipment.EquipmentType));
        }

        private async void OnAllLevelUp(Equipment equipment)
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int currentLevel = equipment.Level;
            EquipmentLevelData currentLevelData = Manager.I.Data.EquipmentLevelDataDict[currentLevel];
            int upgradeCost = currentLevelData.UpgradeCost;
            int item = currentLevelData.UpgradeRequiredItems;
            if (!equipment.IsMaxLevel && userModel.Gold.Value < upgradeCost)
            {
                string message = Manager.I.Data.LocalizationDataDict["NOT_Enought_Gold"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }

            if (!equipment.IsMaxLevel && userModel.GetScrollCount(equipment.EquipmentType) < item)
            {
                string message = Manager.I.Data.LocalizationDataDict["NOT_Enought_Scroll"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }
            
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<EquipmentLevelUpResponseData>(
                    $"/items/equipments/{equipment.UID}/levelUp/all", null, MethodType.PATCH.ToString());
            
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"error {response.message}");
                return;
            }

            UserEquipmentData updatedUserItem = response.data.updatedUserItem;
            userModel.Gold.Value = response.data.gold;
            switch (updatedUserItem.baseEquipmentType)
            {
                case EquipmentType.Weapon:
                    userModel.Inventory.weaponScroll = response.data.weaponScroll;
                    break;
                case EquipmentType.Gloves:
                    userModel.Inventory.glovesScroll = response.data.glovesScroll;
                    break;
                case EquipmentType.Ring:
                    userModel.Inventory.ringScroll = response.data.ringScroll;
                    break;
                case EquipmentType.Belt:
                    userModel.Inventory.beltScroll = response.data.beltScroll;
                    break;
                case EquipmentType.Armor:
                    userModel.Inventory.armorScroll = response.data.armorScroll;
                    break;
                case EquipmentType.Boots:
                    userModel.Inventory.bootsScroll = response.data.bootsScroll;
                    break;
            }

            Equipment equipItem = userModel.FindEquippedItemOrUnEquippedItem(updatedUserItem.userEquipmentId);
            equipItem.SetUserEquipmentData(updatedUserItem);
            _popup.UpdateUI(equipment, userModel.Gold.Value, userModel.GetScrollCount(equipment.EquipmentType));

            var equipmentPopupPresenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            equipmentPopupPresenter.Refresh();
        }

        private void OnOpenLevelDownPopup(Equipment equipment)
        {
            var levelDownPopupPresenter = PresenterFactory.CreateOrGet<LevelDownPopupPresenter>();
            levelDownPopupPresenter.OpenPopup(equipment);
        }

        private async void OnLevelUp(Equipment equipment)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            string userItemId = equipment.UID;
            var response =
                await Manager.I.Web.SendRequest<EquipmentLevelUpResponseData>($"/items/equipments/{userItemId}/levelUp",
                    null, MethodType.PATCH.ToString());
            
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"error {response.message}");
                return;
            }

            UserEquipmentData updatedUserItem = response.data.updatedUserItem;
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.Gold.Value = response.data.gold;
            switch (updatedUserItem.baseEquipmentType)
            {
                case EquipmentType.Weapon:
                    userModel.Inventory.weaponScroll = response.data.weaponScroll;
                    break;
                case EquipmentType.Gloves:
                    userModel.Inventory.glovesScroll = response.data.glovesScroll;
                    break;
                case EquipmentType.Ring:
                    userModel.Inventory.ringScroll = response.data.ringScroll;
                    break;
                case EquipmentType.Belt:
                    userModel.Inventory.beltScroll = response.data.beltScroll;
                    break;
                case EquipmentType.Armor:
                    userModel.Inventory.armorScroll = response.data.armorScroll;
                    break;
                case EquipmentType.Boots:
                    userModel.Inventory.bootsScroll = response.data.bootsScroll;
                    break;
            }

            Equipment equipItem = userModel.FindEquippedItemOrUnEquippedItem(updatedUserItem.userEquipmentId);
            equipItem.SetUserEquipmentData(updatedUserItem);
            _popup.UpdateUI(equipment, userModel.Gold.Value, userModel.GetScrollCount(equipment.EquipmentType));

            var equipmentPopupPresenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            equipmentPopupPresenter.Refresh();
        }

        private async void OnUnquip(Equipment equipment)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            var response = await Manager.I.Web.SendRequest<EquipResponseData>(
                $"/items/equipments/{equipment.UID}/equip?isEquip=false", null, MethodType.PATCH.ToString());
            
            TaskHelper.CompleteTcs();
            if (response.statusCode !=(int) ServerStatusCodeType.Success)
            {
                Debug.Log("Failed equip");
                return;
            }

            UserEquipmentData currentEquippedItem = response.data.currentEquippedItem;
            UserEquipmentData previousEquippedItem = response.data.previousEquippedItem;
            
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.EquipOrUnEquip(currentEquippedItem, previousEquippedItem);
            
            Manager.I.UI.ClosePopup();

            var presenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            presenter.Refresh();
        }

        private async void OnEquip(Equipment equipment)
        {
            var response = await Manager.I.Web.SendRequest<EquipResponseData>(
                $"/items/equipments/{equipment.UID}/equip?isEquip=true", null, MethodType.PATCH.ToString());

            if (response.statusCode !=(int) ServerStatusCodeType.Success)
            {
                Debug.Log("Failed equip");
                return;
            }

            Manager.I.Audio.Play(Sound.SFX, SoundKey.Equipment_Equip);
            UserEquipmentData currentEquippedItem = response.data.currentEquippedItem;
            UserEquipmentData previousEquippedItem = response.data.previousEquippedItem;
            
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.EquipOrUnEquip(currentEquippedItem, previousEquippedItem);
            
            Manager.I.UI.ClosePopup();

            var presenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            presenter.Refresh();
        }

        public void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }
    }
}