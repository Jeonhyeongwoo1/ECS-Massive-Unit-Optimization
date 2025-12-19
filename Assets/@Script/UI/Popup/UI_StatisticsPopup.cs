using System;
using System.Collections.Generic;
using System.Globalization;
using MewVivor.Common;
using MewVivor.UISubItemElement;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    [Serializable]
    public struct TotalDamageInfoData
    {
        public Sprite skillSprite;
        public string skillName;
        public float skillAccumlatedDamage;
        public float skillDamageRatioByTotalDamage;
    }
    
    public class UI_StatisticsPopup : BasePopup
    {
        [SerializeField] private List<UI_SkillDamageItem> _skillDamageItemList;
        [SerializeField] private Button _closeButton;

        public void AddEvent(Action onCloseAction)
        {
            _closeButton.SafeAddButtonListener(()=> onCloseAction.Invoke());
        }
        
        public void UpdateUI(List<TotalDamageInfoData> totalDamageInfoDataList)
        {
            for (int i = 0; i < _skillDamageItemList.Count; i++)
            {
                if (i >= totalDamageInfoDataList.Count)
                {
                    _skillDamageItemList[i].gameObject.SetActive(false);
                    continue;
                }

                TotalDamageInfoData data = totalDamageInfoDataList[i];
                string formattedDamage = data.skillAccumlatedDamage.ToString("N0", CultureInfo.InvariantCulture);
                _skillDamageItemList[i].UpdateUI(
                    data.skillDamageRatioByTotalDamage,
                    formattedDamage,
                    data.skillName,
                    data.skillSprite);

                _skillDamageItemList[i].transform.SetAsFirstSibling();
            }
        }

    }
}