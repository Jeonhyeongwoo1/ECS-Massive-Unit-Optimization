using System;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.UISubItemElement;
using MewVivor.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_EquipmentInfoPopup : BasePopup
    {
        
        [Serializable]
        public struct SkillElementData
        {
            public EquipmentGrade EquipmentGrade;
            public GameObject lockObject;
            public GameObject openSkillObject;
            public TextMeshProUGUI skillDescriptionText;
        }

        [SerializeField] private Button _closeButton;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private Image _equipmentGradeImage;
        [SerializeField] private TextMeshProUGUI _itemGradeText;
        [SerializeField] private Image _levelProgressImage;
        [SerializeField] private TextMeshProUGUI _levelText;

        [SerializeField] private Image _equipmentTypeIconImage;
        [SerializeField] private TextMeshProUGUI _equipmentTypeText;
        [SerializeField] private TextMeshProUGUI _equipmentTypeValueText;
        [SerializeField] private TextMeshProUGUI _equipmentDescriptionText;
        [SerializeField] private TextMeshProUGUI _goldAmountText;
        [SerializeField] private TextMeshProUGUI _scrollAmountText;
        [SerializeField] private Button _unequipButton;
        [SerializeField] private Button _levelupButton;
        [SerializeField] private Button _allLevelUpButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private UI_EquipItem _equipItem;
        [SerializeField] private SkillElementData[] _skillElementDataArray;
        [SerializeField] private Button _refreshButton;

        [SerializeField] private TextMeshProUGUI _levelUpText;
        [SerializeField] private TextMeshProUGUI _allLevelUpText;
        [SerializeField] private OnClickScaler _levelUpClickScaler;
        [SerializeField] private OnClickScaler _allLevelUpClickScaler;

        public void AddEvent(Action onUnEquipAction, Action onLevelUpAction, Action onAllLevelUpAction, Action onEquipAction, Action onRefreshAction)
        {
            _unequipButton.SafeAddButtonListener(()=> onUnEquipAction.Invoke());
            _levelupButton.SafeAddButtonListener(()=> onLevelUpAction.Invoke());
            _allLevelUpButton.SafeAddButtonListener(()=> onAllLevelUpAction.Invoke());
            _equipButton.SafeAddButtonListener(()=> onEquipAction.Invoke());
            _refreshButton.SafeAddButtonListener(()=> onRefreshAction.Invoke());
            _closeButton.SafeAddButtonListener(()=> Manager.I.UI.ClosePopup());
        }

        public void UpdateUI(Equipment equipment, int userGoldAmount, int userTargetScrollAmount)
        {
            DataManager dataManager = Manager.I.Data;

            if (dataManager.LocalizationDataDict.TryGetValue(equipment.EquipmentData.NameTextID, out var nameData))
            {
                _itemNameText.text = nameData.GetValueByLanguage();
            }

            if (dataManager.LocalizationDataDict.TryGetValue(equipment.EquipmentData.DescriptionTextID,
                    out var descriptionData))
            {
                _equipmentDescriptionText.text = descriptionData.GetValueByLanguage();
            }

            int currentLevel = equipment.Level;
            EquipmentLevelData currentLevelData = dataManager.EquipmentLevelDataDict[currentLevel];
            int upgradeCost = currentLevelData.UpgradeCost;
            int item = currentLevelData.UpgradeRequiredItems;

            string level = $"{currentLevel}/{equipment.EquipmentData.Grade_MaxLevel}";
            _levelText.text = level;
            _levelProgressImage.fillAmount = (float)currentLevel / equipment.EquipmentData.Grade_MaxLevel;

            _goldAmountText.text = userGoldAmount < upgradeCost
                ? $"<color=red>{userGoldAmount}</color>/{upgradeCost}"
                : $"{userGoldAmount}/{upgradeCost}";
            _scrollAmountText.text = userTargetScrollAmount < item
                ? $"<color=red>{userTargetScrollAmount}</color>/{item}"
                : $"{userTargetScrollAmount}/{item}";

            _equipmentGradeImage.color = Const.EquipmentUIColors.GetEquipmentGradeColor(equipment.Grade);
            // _equipmentGradeImage.sprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentGrade);
            _itemGradeText.text = Utils.GetEquipmentGrade(equipment.Grade);

            string iconSpriteName = $"{equipment.EquipmentType}_Icon.sprite";
            var sprite = Manager.I.Resource.Load<Sprite>(iconSpriteName);
            _equipmentTypeIconImage.sprite = sprite;

            switch (equipment.EquipmentType)
            {
                case EquipmentType.Weapon:
                case EquipmentType.Gloves:
                case EquipmentType.Ring:
                    _equipmentTypeText.text = "ATK";
                    break;
                case EquipmentType.Belt:
                case EquipmentType.Armor:
                case EquipmentType.Boots:
                    _equipmentTypeText.text = "HP";
                    break;
            }

            _equipmentTypeValueText.text = equipment.GetEquipmentStat().ToString();

            ResourcesManager resourcesManager = Manager.I.Resource;
            Sprite equipmentSprite = resourcesManager.Load<Sprite>(equipment.EquipmentData.Sprite);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipment.EquipmentType}_Icon.sprite");
            Sprite gradeSprite = Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.EquipmentData.Grade);
            _equipItem.UpdateUI(equipmentSprite, equipTypeSprite, currentLevel, gradeSprite, false,
                equipment.GetGradeCount(), false);

            _equipButton.gameObject.SetActive(!equipment.IsEquipped());
            _unequipButton.gameObject.SetActive(equipment.IsEquipped());
            _refreshButton.gameObject.SetActive(equipment.Level > 1);
            bool isPossibleLevelUp =
                !equipment.IsMaxLevel && userGoldAmount >= upgradeCost && userTargetScrollAmount >= item;
            _levelupButton.interactable = isPossibleLevelUp;
            _allLevelUpButton.interactable = isPossibleLevelUp;
            _levelUpText.color = isPossibleLevelUp ? Color.white : Color.red;
            _allLevelUpText.color = isPossibleLevelUp ? Color.white : Color.red;
            _levelUpClickScaler.IsOn = isPossibleLevelUp;
            _allLevelUpClickScaler.IsOn = isPossibleLevelUp;
            UpdateSkillInfo(equipment, dataManager);
            
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void UpdateSkillInfo(Equipment equipment, DataManager dataManager)
        {
            EquipmentGrade equipmentGrade = equipment.EquipmentData.Grade;
            int skillGroupDataId = equipment.EquipmentData.SkillGroupDataId;
            EquipmentSkillGroupData skillGroupData = dataManager.EquipmentSkillGroupDataDict[skillGroupDataId];
            foreach (SkillElementData skillElementData in _skillElementDataArray)
            {
                skillElementData.openSkillObject.SetActive(skillElementData.EquipmentGrade <= equipmentGrade);
                skillElementData.lockObject.SetActive(skillElementData.EquipmentGrade > equipmentGrade);

                int targetSKillId = -1;
                switch (skillElementData.EquipmentGrade)
                {
                    case EquipmentGrade.Uncommon:
                        targetSKillId = skillGroupData.UncommonGradeSkill;
                        break;
                    case EquipmentGrade.Rare:
                        targetSKillId = skillGroupData.RareGradeSkill;
                        break;
                    case EquipmentGrade.Epic:
                        targetSKillId = skillGroupData.EpicGradeSkill;
                        break;
                    case EquipmentGrade.Legendary:
                        targetSKillId = skillGroupData.LegendaryGradeSkill;
                        break;
                }
                
                EquipmentSkillData skillData = dataManager.EquipmentSkillDataDict[targetSKillId];
                LocalizationData localizationData =
                    dataManager.LocalizationDataDict[skillData.DescriptionTextID];
                string descriptionValue = localizationData.GetValueByLanguage();
                string result = null;
                if (skillData.EquipmentSkillType == EquipmentSkillType.AuraDamage)
                {
                    result = string.Format(descriptionValue, skillData.Type_Value1 * 100f, skillData.Type_Value2, skillData.Type_Value3);
                }
                else
                {
                    float value = skillData.Type_Value1 * 100f;
                    result = string.Format(descriptionValue, value);
                }

                skillElementData.skillDescriptionText.text = result;
            }
        }
    }
}