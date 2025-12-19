using System.Collections.Generic;
using MewVivor.Data.Server;
using MewVivor.Interface;
using UniRx;

namespace MewVivor.Model
{
    public class QuestModel : IModel
    {
        public ReactiveProperty<bool> IsPossibleGetRewardQuestOrAchievement = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsPossibleGetRewardQuest = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsPossibleGetRewardAchievement = new ReactiveProperty<bool>();
        
        public List<DailyQuestData> quests;
        public List<DailyQuestStageData> questStages;
        public int totalQuestPoint;
        public List<AchievementInfoData> achievements;

        public void SetQuestData(List<DailyQuestData> quests, List<DailyQuestStageData> questStages,
            int totalQuestPoint, List<AchievementInfoData> achievements)
        {
            DailyQuestData questData = null;
            if (quests != null)
            {
                this.quests = quests;
                questData = quests.Find(v=> v.isCleared && !v.isClaimed);
            }

            DailyQuestStageData dailyQuestStageData = null;
            if (questStages != null)
            {
                this.questStages = questStages;
                dailyQuestStageData = questStages.Find(v => v.isCleared && !v.isClaimed);
            }

            AchievementInfoData achievementsData = null;
            if (achievements != null)
            {
                this.achievements = achievements;
                achievementsData = achievements.Find(v => v.isClearable);
            }
            
            this.totalQuestPoint = totalQuestPoint;
            IsPossibleGetRewardQuest.Value = questData != null || dailyQuestStageData != null;
            IsPossibleGetRewardAchievement.Value = achievementsData != null;
            IsPossibleGetRewardQuestOrAchievement.Value =
                IsPossibleGetRewardQuest.Value || IsPossibleGetRewardAchievement.Value;
        }

        public void SetAchievementData(List<AchievementInfoData> achievements)
        {
            if (achievements != null)
            {
                this.achievements = achievements;
                var achievementsData = achievements.Find(v => v.isClearable);
                IsPossibleGetRewardAchievement.Value = achievementsData != null;
                IsPossibleGetRewardQuestOrAchievement.Value =
                    IsPossibleGetRewardQuest.Value || IsPossibleGetRewardAchievement.Value;
            }
        }

        public void Reset()
        {
            quests = null;
            achievements = null;
            questStages = null;
            totalQuestPoint = 0;
        }
    }
}