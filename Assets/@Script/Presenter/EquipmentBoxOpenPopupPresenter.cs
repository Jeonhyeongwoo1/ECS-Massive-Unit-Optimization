using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class EquipmentBoxOpenPopupPresenter : BasePresenter
    {
        private UI_EquipmentBoxOpenPopup _popup;

        public void OpenPopup(List<Equipment> grantedEquipments)
        {
            _popup = Manager.I.UI.OpenPopup<UI_EquipmentBoxOpenPopup>();
            _popup.ReleaseEquipItem();
            ResourcesManager resourcesManager = Manager.I.Resource;
            if (grantedEquipments == null)
            {
                return;
            }

            foreach (Equipment equipment in grantedEquipments)
            {
                EquipmentType equipmentType = equipment.EquipmentType;
                Sprite equipSprite = resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
                int level = equipment.Level;
                Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                Sprite gradeSprite =
                    Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.Grade);

                var equipItem = Manager.I.UI.AddSubElementItem<UI_EquipItem>(_popup.PivotTransform);
                equipItem.UpdateUI(equipSprite, equipTypeSprite, level, gradeSprite,
                    false, equipment.GetGradeCount());
                equipItem.transform.localScale = Vector3.one;
                equipItem.ActiveLevelText(false);
                equipItem.gameObject.SetActive(false);
                _popup.AddEquipItem(equipItem);
            }

            var equipmentGrade = grantedEquipments[0].Grade;
            LocalizationData data = Manager.I.Data.LocalizationDataDict[grantedEquipments[0].EquipmentData.NameTextID];
            _popup.AddEvent(OnClosePopup);
            _popup.UpdateUI(equipmentGrade, data.GetValueByLanguage());
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