using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class MergeResultPresenter : BasePresenter
    {
        private UserModel _userModel;
        
        public void Initialize(UserModel model)
        {
            _userModel = model;
            Manager.I.Event.AddEvent(GameEventType.ShowMergeResultPopup, OnShowMergeResultPopup);
        }

        private void OnShowMergeResultPopup(object value)
        {
            var newItemUIDList = (List<string>)value;
            if (newItemUIDList.Count > 1) // 한개의 경우만 처리
            {
                return;
            }

            var popup = Manager.I.UI.OpenPopup<UI_MergeResultPopup>();
            string uid = newItemUIDList[0];
            
            Equipment equipment = _userModel.FindEquippedItemOrUnEquippedItem(uid);
            EquipmentData equipmentData = equipment.EquipmentData;
            DataManager dataManager = Manager.I.Data;
            ResourcesManager resourcesManager = Manager.I.Resource;

            string equipName = equipmentData.NameTextID;
            string equipGrade = equipmentData.Grade.ToString();
            Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentData.EquipmentType}_Icon.sprite");
            int level = equipment.Level;
            Color gradeColor = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentData.Grade);
        }
    }
}