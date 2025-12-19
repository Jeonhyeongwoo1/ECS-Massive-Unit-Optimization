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
    public class UI_EquipmentLevelDownSuccessPopup : BasePopup
    {
        [SerializeField] private UI_EquipItem _equipItem;
        [SerializeField] private Image _scrollImage;
        [SerializeField] private TextMeshProUGUI _scollAmountText;

        public void UpdateUI(Equipment equipment, int scrollAmount)
        {
            ResourcesManager resourcesManager = Manager.I.Resource;
            Sprite equipmentSprite = resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipment.EquipmentType}_Icon.sprite");
            Sprite gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentData.Grade);
            _equipItem.UpdateUI(equipmentSprite, equipTypeSprite, equipment.Level, gradeSprite, false,
                equipment.GetGradeCount(), false);
            
            _scollAmountText.text = $"X{scrollAmount}";
            Sprite scrollSprite = Utils.GetScrollSprite(equipment.EquipmentType);
            _scrollImage.sprite = scrollSprite;
        }

    }
}