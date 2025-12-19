using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.InGame.Skill;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_PausePopup : BasePopup
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _exitGameButton;
        [SerializeField] private Button _bgmButton;
        [SerializeField] private Button _sfxButton;
        [SerializeField] private Button _statisticsButton;
        [SerializeField] private GameObject _bgmOffImageObject;
        [SerializeField] private GameObject _sfxOffImageObject;

        [SerializeField] private UI_SkillSlotItem[] _attackSkillSlotItemArray;
        [SerializeField] private UI_SkillSlotItem[] _passiveSkillSlotItemArray;
        [SerializeField] private TextMeshProUGUI _playerLevelText;
        [SerializeField] private Image _playerLevelImage;

        public void AddEvent(Action onResumeAction, Action onExitGameAction, Action<bool> onActivateSoundAction,
            Action onShowStatisticsAction)
        {
            _resumeButton.SafeAddButtonListener(() => onResumeAction?.Invoke());
            _exitGameButton.SafeAddButtonListener(() => onExitGameAction?.Invoke());
            _statisticsButton.SafeAddButtonListener(() => onShowStatisticsAction?.Invoke());
            _bgmButton.SafeAddButtonListener(() => onActivateSoundAction?.Invoke(true));
            _sfxButton.SafeAddButtonListener(() => onActivateSoundAction.Invoke(false));
        }

        public void UpdateUI(List<BaseAttackSkill> attackSkillList, List<BasePassiveSkill> passiveSkillList, string playerLevel, float playerLevelRatio)
        {
            for (int i = 0; i < Const.MAX_AttackSKiLL_Level; i++)
            {
                if (attackSkillList.Count > i)
                {
                    BaseAttackSkill attackSkill = attackSkillList[i];
                    _attackSkillSlotItemArray[i].UpdateUI(true, attackSkill.AttackSkillData, attackSkill.CurrentLevel);
                }
                else
                {
                    _attackSkillSlotItemArray[i].UpdateUI(false);
                }
            }
            
            for (int i = 0; i < Const.MAX_AttackSKiLL_Level; i++)
            {
                if (passiveSkillList.Count > i)
                {
                    BasePassiveSkill passiveSkill = passiveSkillList[i];
                    _passiveSkillSlotItemArray[i].UpdateUI(true, passiveSkill.PassiveSkillData, passiveSkill.CurrentLevel);
                }
                else
                {
                    _passiveSkillSlotItemArray[i].UpdateUI(false);
                }
            }

            _playerLevelImage.fillAmount = playerLevelRatio;
            _playerLevelText.text = playerLevel;
            UpdateSoundUI(Manager.I.IsOnBGM, Manager.I.IsOnSfx);
        }

        public void UpdateSoundUI(bool isOnBGM, bool isOnSFX)
        {
            _bgmOffImageObject.SetActive(!isOnBGM);
            _sfxOffImageObject.SetActive(!isOnSFX);
        }
    }
}