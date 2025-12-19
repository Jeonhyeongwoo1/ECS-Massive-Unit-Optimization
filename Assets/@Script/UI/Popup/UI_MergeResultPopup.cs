using System.Collections;
using System.Collections.Generic;
using MewVivor.OutGame.UI;
using MewVivor.Popup;
using MewVivor.Presenter;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;

namespace MewVivor.Popup
{
    public class UI_MergeResultPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _equipmentNameValueText;
        [SerializeField] private TextMeshProUGUI _equipmentGradeValueText;
        [SerializeField] private UI_EquipItem _mergeResultEquipItem;
        [SerializeField] private UI_MergeOptionResult _mergeOptionResult;

        // public void UpdateUI(string equipName, string equipGrade, Sprite equipSprite, Sprite equipTypeSprite, int level,
        //     Color gradeColor, List<MergeOptionResultData> mergeOptionResultDataList, string improveOptionValue)
        // {
        //     _equipmentNameValueText.text = equipName;
        //     _equipmentGradeValueText.text = equipGrade;
        //     _mergeResultEquipItem.UpdateUI(equipSprite, equipTypeSprite, level,
        //         gradeColor);
        //     _mergeOptionResult.UpdateMergeOption(mergeOptionResultDataList, improveOptionValue, null);
        // }
    }
}