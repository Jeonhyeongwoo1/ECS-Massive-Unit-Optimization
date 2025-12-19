using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Skill;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class SkillSelectPresenter : BasePresenter
    {
        private UI_SkillSelectPopup _popup;
        private bool _isLevelUp = false;
        
        public void OpenSkillSelectPopup(bool isLevelUp)
        {
            _popup = Manager.I.UI.OpenPopup<UI_SkillSelectPopup>();
            Debug.Log("OpenSkillSelectPopup");
            _popup.AddEvent(OnRefreshSkillList, OnUpgradeOrAddNewSkill);
            UpdateSkillList();
            TimeScaleHandler.RequestPause();
            _isLevelUp = isLevelUp;
        }
        
        private void UpdateSkillList()
        {
            PlayerController player = Manager.I.Object.Player;
            SkillBook skillBook = player.SkillBook;
            List<BaseSkillData> recommendSkillList = skillBook.GetRecommendSkillList();
            //스킬이 0개인 경우에는 체력 아이템 1종 등장
            if (recommendSkillList.Count == 0)
            {
                Debug.Log($"recommend skill list {recommendSkillList.Count}");
                _popup.UpdateUI(null, null, null ,false, false);
                return;
            }

            List<string> list = skillBook.ActivateAttackSkillList.Select(x => x.AttackSkillData.IconLabel).ToList();
            List<Sprite> attackSkillIconSpriteList = new List<Sprite>(list.Count);
            foreach (string iconLabel in list)
            {
                var sprite = Manager.I.Resource.Load<Sprite>(iconLabel);
                attackSkillIconSpriteList.Add(sprite);
            }

            list = skillBook.ActivatePassiveSkillList.Select(x => x.PassiveSkillData.IconLabel).ToList();
            List<Sprite> passiveSkillIconSpriteList = new List<Sprite>(list.Count);
            foreach (string iconLabel in list)
            {
                var sprite = Manager.I.Resource.Load<Sprite>(iconLabel);
                passiveSkillIconSpriteList.Add(sprite);
            }

            PlayerModel model = ModelFactory.CreateOrGetModel<PlayerModel>();
            bool isPossibleRefresh = model.SkillSelectRefreshUseCount.Value < Const.SkillSelectRefreshMaxCount;
            bool isFreeRefresh = model.SkillSelectRefreshUseCount.Value < 1;
            _popup.UpdateUI(recommendSkillList,
                attackSkillIconSpriteList,
                passiveSkillIconSpriteList,
                isFreeRefresh,
                isPossibleRefresh);
        }

        private void OnRefreshSkillList()
        {
            PlayerModel model = ModelFactory.CreateOrGetModel<PlayerModel>();
            if (model.SkillSelectRefreshUseCount.Value > Const.SkillSelectRefreshMaxCount)
            {
                return;
            }

            if (model.SkillSelectRefreshUseCount.Value >= 1)
            {
                Manager.I.Ads.ShowRewardAd(() =>
                {
                    UI_BlockCanvas.I.ShowAndHideBlockCanvas(true);
                    DOVirtual.DelayedCall(0.1f, () =>
                    {

                        UI_BlockCanvas.I.ShowAndHideBlockCanvas(false);
                        Time.timeScale = 0;
                    });
                    model.SkillSelectRefreshUseCount.Value++;
                    UpdateSkillList();
                }, () => { Manager.I.UI.OpenSystemPopup(Manager.I.Data.LocalizationDataDict["Failed_ads"].GetValueByLanguage()); });
            }
            else
            {
                model.SkillSelectRefreshUseCount.Value++;
                UpdateSkillList();
            }
        }

        private void OnUpgradeOrAddNewSkill(int skillId)
        {
            TimeScaleHandler.ReleasePause();
            DataManager dataManager = Manager.I.Data;
            PlayerController player = Manager.I.Object.Player;
            if (dataManager.AttackSkillDict.TryGetValue(skillId, out AttackSkillData attackSkillData))
            {
                var skill = player.SkillBook.ActivateAttackSkillList.Find(v =>
                    v.AttackSkillType == attackSkillData.AttackSkillType);
                if (skill != null)
                {
                    skillId = skill.AttackSkillData.DataId;
                }
            }

            if (dataManager.PassiveSkillDataDict.TryGetValue(skillId, out PassiveSkillData passiveSkillData))
            {
                var skill = player.SkillBook.ActivatePassiveSkillList.Find(v =>
                    v.PassiveSkillType == passiveSkillData.PassiveSkillType);
                if (skill != null)
                {
                    skillId = skill.PassiveSkillData.DataId;
                }
            }

            if (skillId == Const.ID_POTION)
            {
                float healAmount = player.MaxHP.Value * 0.5f;
                player.Heal(healAmount);
                player.LevelUp();
            }
            else
            {
                player.OnUpgradeOrAddNewSkill(skillId, _isLevelUp);
            }
            //View
            Manager.I.UI.ClosePopup();
            _isLevelUp = false;
        }
    }
}