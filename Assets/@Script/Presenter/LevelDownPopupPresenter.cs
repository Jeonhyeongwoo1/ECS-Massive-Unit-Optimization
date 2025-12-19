using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Util;

namespace MewVivor.Presenter
{
    public class LevelDownPopupPresenter : BasePresenter
    {
        private UI_LevelDownPopup _popup;
        private Equipment _equipment;
        
        public void OpenPopup(Equipment equipment)
        {
            _equipment = equipment;
            _popup = Manager.I.UI.OpenPopup<UI_LevelDownPopup>();
            _popup.AddEvent(OnEquipLevelDown, OnClosePopup);
            int scrollAmount = CalculateUpgradeRequiredScroll(equipment.Level);
            _popup.UpdateUI(equipment, scrollAmount);
        }

        private void OnClosePopup()
        {
            Manager.I.UI.ClosePopup();
        }

        private async void OnEquipLevelDown()
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            string userItemId = _equipment.UID;
            var response =
                await Manager.I.Web.SendRequest<EquipmentLevelDownResponseData>(
                    $"/items/equipments/{userItemId}/initialize", null, MethodType.PATCH.ToString());

            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                return;
            }
            
            UserEquipmentData updatedUserItem = response.data.updatedUserItem;
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.Gold.Value = response.data.gold;
            int scrollAmount = CalculateUpgradeRequiredScroll(_equipment.Level);
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

            Manager.I.UI.ClosePopup();
            
            var equipmentPopupPresenter = PresenterFactory.CreateOrGet<EquipmentInfoPopupPresenter>();
            equipmentPopupPresenter.ClosePopup();
            
            var equipmentLevelDownSuccessPopupPresenter =
                PresenterFactory.CreateOrGet<EquipmentLevelDownSuccessPopupPresenter>();
            equipmentLevelDownSuccessPopupPresenter.OpenEquipmentLevelDownSuccessPopup(equipItem, scrollAmount);
            _equipment = null;
        }

        private int CalculateUpgradeRequiredScroll(int currentLevel)
        {
            int result = 0;
            foreach (var (key, levelData) in Manager.I.Data.EquipmentLevelDataDict)
            {
                if (key == 1)
                {
                    continue;
                }
                
                if (key <= currentLevel)
                {
                    result += levelData.UpgradeRequiredItems;
                }
            }

            return result;
        }
    }
}