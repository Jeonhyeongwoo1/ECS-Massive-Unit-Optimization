using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MewVivor.Popup
{
    public class UI_EquipmentPopup : BasePopup
    {
        [Serializable]
        public struct EquipmentElement
        {
            public EquipmentType equipmentType;
            public UI_EquipItem equipItem;
        }

        public Transform EquipInventoryObject => _equipInventoryObject;
        
        [SerializeField] private List<EquipmentElement> _equippedItemList;
        [SerializeField] private TextMeshProUGUI _totalAttackDamageText;
        [SerializeField] private TextMeshProUGUI _totalHealthText;
        [SerializeField] private Transform _equipInventoryObject;
        [SerializeField] private Button _sortButton;
        [SerializeField] private Button _mergeButton;
        [SerializeField] private TextMeshProUGUI _sortByLevelText;
        [SerializeField] private TextMeshProUGUI _sortQualityText;
        [SerializeField] private GameObject _sortByLevelActiveObject;
        [SerializeField] private GameObject _sortQualityActiveObject;
        [SerializeField] private GameObject _mergeRedDotObject;

        public void AddEvent(Action onSortEquipItemAction, Action onShowMergePopupAction)
        {
            _sortButton.SafeAddButtonListener(onSortEquipItemAction.Invoke);
            _mergeButton.SafeAddButtonListener(onShowMergePopupAction.Invoke);
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.IsPossibleMerge.Subscribe(x=>
            {
                _mergeButton.gameObject.SetActive(x);
                _mergeRedDotObject.SetActive(x);
            }).AddTo(this);
        }
        
        public void ReleaseUneuippedItem()
        {
            var childs = Utils.GetChildComponent<UI_EquipItem>(_equipInventoryObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_EquipItem equipItem in childs)
            {
                equipItem.Release();
            }
        }

        public List<EquipmentElement> GetEquipmentItemList()
        {
            return _equippedItemList;
        }
        
        public void UpdateUI(string totalAttackDamageValue, string totalHealthValue, EquipmentSortType equipmentSortType)
        {
            _totalAttackDamageText.text = totalAttackDamageValue;
            _totalHealthText.text = totalHealthValue;
            _sortByLevelText.color = equipmentSortType == EquipmentSortType.Level
                ? Const.MergeSortTypeActiveColor
                : Const.MergeSortTypeDeactiveColor;
            _sortQualityText.color = equipmentSortType == EquipmentSortType.Grade
                ? Const.MergeSortTypeActiveColor
                : Const.MergeSortTypeDeactiveColor;
            _sortByLevelActiveObject.SetActive(equipmentSortType == EquipmentSortType.Level);
            _sortQualityActiveObject.SetActive(equipmentSortType == EquipmentSortType.Grade);
        }
    }
}