using System;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_LevelDownPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _levelDownButton;
        [SerializeField] private UI_EquipItem _equipItem;
        [SerializeField] private Image _equipmentIconImage;
        [SerializeField] private Image _equipmentTypeIconImage;
        [SerializeField] private Image _equipmentImage;

        [SerializeField] private TextMeshProUGUI _scollAmountText;
        [SerializeField] private Image _scrollImage;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        public void AddEvent(Action onLevelDownAction, Action onCloseAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _levelDownButton.SafeAddButtonListener(onLevelDownAction.Invoke);
        }

        public void UpdateUI(Equipment equipment, int scrollAmount)
        {
            ResourcesManager resourcesManager = Manager.I.Resource;
            Sprite equipmentSprite = resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipment.EquipmentType}_Icon.sprite");
            Sprite gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentData.Grade);
            _equipItem.UpdateUI(equipmentSprite, equipTypeSprite, equipment.Level, gradeSprite, false,
                equipment.GetGradeCount(), false);

            _equipmentIconImage.sprite = equipmentSprite;
            _equipmentTypeIconImage.sprite = equipTypeSprite;
            _equipmentImage.sprite = gradeSprite;
            _scollAmountText.text = $"X{scrollAmount}";
            Sprite scrollSprite = Utils.GetScrollSprite(equipment.EquipmentType);
            _scrollImage.sprite = scrollSprite;
        }
    }
}