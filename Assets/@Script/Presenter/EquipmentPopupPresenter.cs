using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class EquipmentPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private UI_EquipmentPopup _popup;
        private ResourcesManager _resourcesManager;
        private DataManager _dataManager;
        private UIManager _uiManager;
        private EquipmentSortType _equipmentSortType = EquipmentSortType.Level;
        
        public void Initialize()
        {
            _userModel = ModelFactory.CreateOrGetModel<UserModel>();
            _resourcesManager = Manager.I.Resource;
            _dataManager = Manager.I.Data;
            _uiManager = Manager.I.UI;
        }

        private void OnUpdatedEquipment(object value)
        {
            Refresh();
        }

        public void OpenPopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_EquipmentPopup>();
            _popup.Initialize();
            _popup.AddEvent(OnSortEquipItem, OnShowMergePopup);
            Refresh();
        }
        
        public void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }
        
        public void Refresh()
        {
            UpdateEquippedItem();
            MakeUnEquipItemInventory();

            int totalHP = _userModel.GetTotalEquipmentStat().hp;
            int totalAttackDamage = _userModel.GetTotalEquipmentStat().atk;
            _popup.UpdateUI(totalAttackDamage.ToString(), totalHP.ToString(), _equipmentSortType);
        }

        private void MakeUnEquipItemInventory()
        {
            _popup.ReleaseUneuippedItem();
            List<Equipment> unequipItemList = _userModel.GetUnequipItemList();
            List<Equipment> sortedEquipmentList = null;
            if (_equipmentSortType == EquipmentSortType.Grade)
            {
                sortedEquipmentList = unequipItemList
                        .OrderByDescending(x => x.EquipmentData.Grade)
                        .ThenBy(x => x.IsEquipped())
                        .ThenBy(x => x.Level)
                        .ThenBy(x => x.EquipmentData.EquipmentType)
                        .ToList();
            }
            else if (_equipmentSortType == EquipmentSortType.Level)
            {
                sortedEquipmentList = unequipItemList
                    .OrderByDescending(x => x.Level)
                    .ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.EquipmentData.Grade)
                    .ThenBy(x => x.EquipmentData.EquipmentType)
                    .ToList();
            }
            
            if (sortedEquipmentList != null)
            {
                foreach (Equipment equipment in sortedEquipmentList)
                {
                    AddUIUnEquipItem(equipment);
                }
            }   
        }   

        private void OnSortEquipItem()
        {
            _equipmentSortType = _equipmentSortType switch
            {
                EquipmentSortType.Grade => EquipmentSortType.Level,
                EquipmentSortType.Level => EquipmentSortType.Grade,
                _ => _equipmentSortType
            };

            Refresh();
        }

        private void UpdateEquippedItem()
        {
            List<UI_EquipmentPopup.EquipmentElement> uiEquipItemList = _popup.GetEquipmentItemList();
            foreach (UI_EquipmentPopup.EquipmentElement uiEquipItem in uiEquipItemList)
            {
                EquipmentType equipmentType = uiEquipItem.equipmentType;
                Equipment equipment = _userModel.FindEquippedEquipment(equipmentType);
                int level = 0;
                Sprite sprite = null;
                Sprite gradeSprite = null;
                if (equipment != null)
                {
                    sprite = _resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
                    level = equipment.Level;
                    gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentData.Grade);
                }
                else
                {
                    gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(EquipmentGrade.Common);
                }

                Sprite equipTypeSprite = _resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                uiEquipItem.equipItem.UpdateUI(sprite, equipTypeSprite, level, gradeSprite, false,
                    equipment?.GetGradeCount() ?? 0, equipment?.IsPossibleLevelUp() ?? false);
                uiEquipItem.equipItem.ActiveEquipment(equipment != null);
                if (equipment != null)
                {
                    uiEquipItem.equipItem.AddEvent(() => OnClickEquipItem(equipment.UID));
                }
                
                uiEquipItem.equipItem.ActivateScaler(equipment != null);
            }
        }

        private void AddUIUnEquipItem(Equipment equipment)
        {
            EquipmentType equipmentType = equipment.EquipmentData.EquipmentType;
            Transform parentTransform = _popup.EquipInventoryObject;
            UI_EquipItem equipItem = _uiManager.AddSubElementItem<UI_EquipItem>(parentTransform);
            Sprite sprite = _resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
            int level = equipment.Level;
            Sprite equipTypeSprite = _resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
            Sprite gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentData.Grade);
            equipItem.UpdateUI(sprite, equipTypeSprite, level, gradeSprite, false, equipment.GetGradeCount(), false);
            equipItem.AddEvent(()=> OnClickEquipItem(equipment.UID));
            equipItem.ActivateScaler(true);
            equipItem.transform.SetAsLastSibling();
            equipItem.transform.localScale = Vector3.one; 
        }
        

        private void OnClickEquipItem(string id)
        {
            var equipmentInfoPopupPresenter = PresenterFactory.CreateOrGet<EquipmentInfoPopupPresenter>();
            equipmentInfoPopupPresenter.ShowPopup(id);
        }
        
        private void OnShowMergePopup()
        {
            var mergePopupPresenter = PresenterFactory.CreateOrGet<EquipmentMergePopupPresenter>();
            mergePopupPresenter.OpenEquipmentMergePopup();
        }
    }
}