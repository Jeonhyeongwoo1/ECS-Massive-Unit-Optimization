using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.UISubItemElement
{
    public class UI_ShopEquipItem : UI_EquipItem
    {

        public void UpdateUI(Sprite equipSprite, Sprite equipTypeSprite, Sprite gradeSprite)
        {
            _equipImage.sprite = equipSprite;
            _equipTypeImage.sprite = equipTypeSprite;
            _equipmentGradeBackgroundImage.sprite = gradeSprite;
        }
    }
}