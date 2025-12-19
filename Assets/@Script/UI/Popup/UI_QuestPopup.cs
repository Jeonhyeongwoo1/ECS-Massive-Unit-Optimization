using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MewVivor.Popup
{
    public class UI_QuestPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _dailyQuestButton;
        [SerializeField] private Button _achievementButton;
        [SerializeField] private GameObject _dailyQuestObject;
        [SerializeField] private GameObject _achievementObject;
        [SerializeField] private Image _dailyQuestProgressbarImage;
        [SerializeField] private TextMeshProUGUI _dailyQuestRefreshTimerText;
        [SerializeField] private List<UI_QuestProgressRewardItem> _uiQuestProgressRewardItemList;
        [SerializeField] private ScrollRect _questScrollRect;
        
        //achievement
        [SerializeField] private ScrollRect _achievementScrollRect;
        [SerializeField] private GameObject _achievementRedDotObject;
        [SerializeField] private GameObject _questRedDotObject;
        
        private Action<int> _onGetQuestRewardAction;
        private Action<int> _onGetQuestStageRewardAction;
        private Action<int> _onGetAchievementRewardAction;
        private CompositeDisposable _subscriptionDisposables;

        public void AddEvent(Action onCloseAction, Action onDailyQuestAction, Action onAchievementAction,
            Action<int> onGetQuestRewardAction, Action<int> onGetQuestStageRewardAction,
            Action<int> onGetAchievementRewardAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _dailyQuestButton.SafeAddButtonListener(onDailyQuestAction.Invoke);
            _achievementButton.SafeAddButtonListener(onAchievementAction.Invoke);

            _onGetQuestRewardAction = onGetQuestRewardAction;
            _onGetQuestStageRewardAction = onGetQuestStageRewardAction;
            _onGetAchievementRewardAction = onGetAchievementRewardAction;

            if (_subscriptionDisposables == null)
            {
                _subscriptionDisposables = new CompositeDisposable();
                var model = ModelFactory.CreateOrGetModel<QuestModel>();
                model.IsPossibleGetRewardQuest.Subscribe(x => { _questRedDotObject.SetActive(x); })
                    .AddTo(_subscriptionDisposables);
                model.IsPossibleGetRewardAchievement.Subscribe(x => { _achievementRedDotObject.SetActive(x); })
                    .AddTo(_achievementRedDotObject);
            }
        }

        public void UpdateUI(QuestTabType questTabType, QuestModel questModel)
        {
            _dailyQuestObject.SetActive(questTabType == QuestTabType.DailyQuest);
            _achievementObject.SetActive(questTabType == QuestTabType.Achievement);
            
            if (questTabType == QuestTabType.DailyQuest)
            {
                List<DailyQuestData> quests = questModel.quests;
                List<DailyQuestStageData> questStages = questModel.questStages;
                int totalQuestPoints = questModel.totalQuestPoint;
                UpdateDailyQuestUI(quests, questStages, totalQuestPoints);
            }
            else
            {
                List<AchievementInfoData> achievementDataList = questModel.achievements;
                UpdateAchievement(achievementDataList);
            }
        }

        private void UpdateAchievement(List<AchievementInfoData> achievementDataList)
        {
            ReleaseAchievementElementItem();

            DataManager dataManager = Manager.I.Data;
            for (var i = 0; i < achievementDataList.Count; i++)
            {
                AchievementInfoData achievementInfoData = achievementDataList[i];
                int id = int.Parse(achievementInfoData.id);
                AchievementData achievementData = dataManager.AchievementDataDict[id];
                
                string descKey = achievementData.DescriptionTextKey;
                string description = "";
                if (dataManager.LocalizationDataDict.TryGetValue(descKey, out LocalizationData localizationData))
                {
                    description = localizationData.GetValueByLanguage();
                }
                
                int resultGoal = achievementInfoData.clearValue;
                int currentGoal = achievementInfoData.currentValue;
                int lifeTime = (int) AchievementMissionTarget.InfiniteModeLifeTime;
                if (achievementInfoData.id == lifeTime.ToString())
                {
                    currentGoal /= 1000;
                }
                
                float ratio = (float)currentGoal / resultGoal;
                string progressDesc = $"({currentGoal}/{resultGoal})";
                ItemData itemData = dataManager.ItemDataDict[achievementData.Reward_Id];
                Sprite sprite = null;
                if (itemData.ID == Const.ID_JEWEL)
                {
                    sprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                }
                else
                {
                    sprite = Manager.I.Resource.Load<Sprite>(itemData.SpriteName);
                }
                var item = Manager.I.UI.AddSubElementItem<UI_AchievementItem>(_achievementScrollRect.content);
                item.AddEvent(() => _onGetAchievementRewardAction.Invoke(id));
                item.UpdateUI(description, ratio, sprite, achievementData.Reward_Amount, achievementInfoData.isClearable, progressDesc);
                item.transform.localScale = Vector3.one;
                if (achievementInfoData.isClearable)
                {
                    item.transform.SetAsFirstSibling();
                }
                else
                {
                    item.transform.SetAsLastSibling();
                }
            }
        }

        private void ReleaseAchievementElementItem()
        {
            var childs = _achievementScrollRect.content.GetComponentsInChildren<UI_AchievementItem>();
            foreach (UI_AchievementItem uiQuestElementItem in childs)
            {
                Manager.I.Pool.ReleaseObject(nameof(UI_AchievementItem), uiQuestElementItem.gameObject);
            }
        }

        private void UpdateDailyQuestUI(List<DailyQuestData> quests, List<DailyQuestStageData> questStages,
            int totalQuestPoints)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime tomorrowUtcReset =
                new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
            TimeSpan remain = tomorrowUtcReset - utcNow;
            _dailyQuestRefreshTimerText.text = $"{remain.Hours:D2}h {remain.Minutes:D2}m";
            DataManager dataManager = Manager.I.Data;
            
            UI_QuestProgressRewardItem firstItem = _uiQuestProgressRewardItemList[0];
            firstItem.UpdateUI(true, "0", false, false, null);
            for (int i = 0; i < questStages.Count; i++)
            {
                DailyQuestStageData questStageData = questStages[i];
                UI_QuestProgressRewardItem rewardItem = _uiQuestProgressRewardItemList[i + 1];
                DailyQuestRewardData dailyQuestRewardData =
                    dataManager.DailyQuestRewardDataDict[questStageData.questStageId];
                rewardItem.AddEvent(() => _onGetQuestStageRewardAction.Invoke(questStageData.questStageId));

                int rewardId = dailyQuestRewardData.Reward_Id;
                int rewardAmount = dailyQuestRewardData.Reward_Amount;
                ItemData itemData = Manager.I.Data.ItemDataDict[rewardId];
                TooltipRewardData data = new TooltipRewardData
                {
                    sprite = Manager.I.Resource.Load<Sprite>(itemData.SpriteName),
                    rewardAmount = rewardAmount
                };

                rewardItem.UpdateUI(false, dailyQuestRewardData.NeedDailyPointReward_Id.ToString(), questStageData.isClaimed,
                    questStageData.isCleared,
                    new List<TooltipRewardData>() { data });
            }

            ReleaseQuestElementItem();
            for (var i = 0; i < quests.Count; i++)
            {
                DailyQuestData questData = quests[i];
                if (questData.isClaimed)
                {
                    continue;
                }
                
                QuestData dailyQuestData = dataManager.QuestDataDict[questData.questMissionId];
                string descKey = dailyQuestData.DescriptionTextKey;
                string description = "";
                if (dataManager.LocalizationDataDict.TryGetValue(descKey, out LocalizationData localizationData))
                {
                     description = localizationData.GetValueByLanguage();
                }

                float ratio = (float)questData.progress / (float) dailyQuestData.QuestMissionTargetValue;
                string progressDesc = $"({questData.progress}/{dailyQuestData.QuestMissionTargetValue})";
                ItemData itemData = dataManager.ItemDataDict[dailyQuestData.Reward_Id];
                Sprite sprite = Manager.I.Resource.Load<Sprite>(itemData.SpriteName);
                var item = Manager.I.UI.AddSubElementItem<UI_QuestElementItem>(_questScrollRect.content);
                item.AddEvent(()=>_onGetQuestRewardAction.Invoke(questData.questMissionId));
                item.UpdateUI(description, ratio, sprite, dailyQuestData.Reward_Amount, questData.isCleared, progressDesc);
                item.transform.localScale = Vector3.one;
                if (questData.isCleared)
                {
                    item.transform.SetAsFirstSibling();
                }
                else
                {
                    item.transform.SetAsLastSibling();
                }
            }

            DailyQuestStageData lastQuestStageData = questStages.LastOrDefault();
            DailyQuestRewardData lastDailyQuestRewardData = Manager.I.Data.DailyQuestRewardDataDict[lastQuestStageData.questStageId];
            float result = (float)totalQuestPoints / lastDailyQuestRewardData.NeedDailyPointReward_Id;
            _dailyQuestProgressbarImage.fillAmount = result;
        }

        private void ReleaseQuestElementItem()
        {
            var childs = _questScrollRect.content.GetComponentsInChildren<UI_QuestElementItem>();
            foreach (UI_QuestElementItem uiQuestElementItem in childs)
            {
                Manager.I.Pool.ReleaseObject(nameof(UI_QuestElementItem), uiQuestElementItem.gameObject);
            }
        }

        private void OnDestroy()
        {
            _subscriptionDisposables?.Dispose();
            _subscriptionDisposables = null;
        }
    }
}