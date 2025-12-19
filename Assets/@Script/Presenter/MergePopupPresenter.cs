using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    [Serializable]
    public class MergeOptionResultData
    {
        public EquipAbilityStatType equipAbilityStatType;
        public float beforeValue;
        public float afterValue;
    }

    public class MergePopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private Dictionary<Equipment, UI_EquipItem> _inventoryEquipItemDict = new();
        private List<Equipment> _equipmentList;
        private UI_MergePopup _mergePopup;
        private ResourcesManager _resourcesManager;
        private UIManager _uiManager;
        private Equipment _selectedResultMergeEquipment;
        private Equipment _selectMergeCostFirstEquipItem;
        private Equipment _selectMergeCostSecondEquipItem;
        private EquipmentSortType _equipmentSortType;
        
        public void Initialize(UserModel model)
        {
            _userModel = model;
            _resourcesManager = Manager.I.Resource;
            _uiManager = Manager.I.UI;
            
            Manager.I.Event.AddEvent(GameEventType.ShowMergePopup, OnShowMergePopup);
        }

        private void OnClosePopup()
        {
            _selectedResultMergeEquipment = null;
            _selectMergeCostFirstEquipItem = null;
            _selectMergeCostSecondEquipItem = null;
            
            _uiManager.ClosePopup();
        }
        
        private void OnShowMergePopup(object value)
        {
            _mergePopup = _uiManager.OpenPopup<UI_MergePopup>();
            _mergePopup.onReleaseSelectedEquipment = OnReleaseSelectedResultMergeEquipment;
            _mergePopup.onReleaseSelectedCostEquipment = OnReleaseSelectedCostEquipment;
            _mergePopup.onMergeAction = OnMergeEquipment;
            _mergePopup.onClosePopupAction = OnClosePopup;
            _mergePopup.onSortEquipItemAction = OnSortEquipItemAction;
            _mergePopup.AddEvents();

            // AddEquipmentList();
            _equipmentSortType = EquipmentSortType.Grade;
            SortEquipItem(_equipmentSortType);
            _mergePopup.UpdateMergeResultEquipItem(false, true, true, null, null, 0, Color.white);
        }

        private void SortEquipItem(EquipmentSortType equipmentSortType)
        {
            List<Equipment> sortedEquipmentList = null;
            if (equipmentSortType == EquipmentSortType.Grade)
            {
                sortedEquipmentList = _equipmentList.OrderBy(x => x.EquipmentData.Grade).ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.Level).ThenBy(x => x.EquipmentData.EquipmentType).ToList();
            }
            else if (equipmentSortType == EquipmentSortType.Level)
            {
                sortedEquipmentList = _equipmentList.OrderBy(x=>x.Level).ThenBy(x => x.IsEquipped())
                    .ThenBy(x => x.EquipmentData.Grade).ThenBy(x => x.EquipmentData.EquipmentType).ToList();
            }

            _equipmentList = sortedEquipmentList;
            string type = _equipmentSortType == EquipmentSortType.Grade ? "레벨 순" : "등급 순";
            _mergePopup.SetSortTypeText(type);            
            RefreshEquipItemUI();
        }
        
        private void OnSortEquipItemAction()
        {
            if (_equipmentSortType == EquipmentSortType.Grade)
            {
                _equipmentSortType = EquipmentSortType.Level;
            }
            else if(_equipmentSortType == EquipmentSortType.Level)
            {
                _equipmentSortType = EquipmentSortType.Grade;
            }
            
            SortEquipItem(_equipmentSortType);
        }
        
        // private void AddEquipmentList()
        // {
        //     _equipmentList ??= new List<Equipment>();
        //
        //     if (_equipmentList.Count > 0)
        //     {
        //         _equipmentList.Clear();
        //     }
        //
        //     _equipmentList = _userModel.EquippedItemDataList.Value.Concat(_userModel.UnEquippedItemDataList.Value).ToList();
        // }

        private async void OnMergeEquipment()
        {
           
        }

        private void Reset()
        {
            _selectedResultMergeEquipment = null;
            _selectMergeCostFirstEquipItem = null;
            _selectMergeCostSecondEquipItem = null;
            _mergePopup.UpdateMergeResultEquipItem(false, true, true, null, null, 0, Color.white);
            _mergePopup.RestEquipMergeCostItem(true, true);
        }

        private void OnReleaseSelectedResultMergeEquipment()
        {
            if (_selectedResultMergeEquipment == null)
            {
                return;
            }

            Reset();
            SortEquipItem(_equipmentSortType);
        }

        private void OnReleaseSelectedCostEquipment(bool isFirstItem)
        {
            Equipment targetEquipment = isFirstItem ? _selectMergeCostFirstEquipItem : _selectMergeCostSecondEquipItem;
            if (targetEquipment == null)
            {
                return;
            }
            
            if (isFirstItem)
            {
                _selectMergeCostFirstEquipItem = null;
                _mergePopup.RestEquipMergeCostItem(true, false);
            }
            else
            {
                _selectMergeCostSecondEquipItem = null;
                _mergePopup.RestEquipMergeCostItem(false, true);
            }
            
            SortEquipItem(_equipmentSortType);
        }

        private void RefreshEquipItemUI()
        {
            _inventoryEquipItemDict.Clear();
            _mergePopup.ReleaseEquipItem();
        }

        private void OnClickEquipItem(Equipment targetEquipment)
        {
            SortEquipItem(_equipmentSortType);
        }
        
    }
}