using System;
using Cysharp.Threading.Tasks;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Util;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class QuestPopupPresenter : BasePresenter
    {
        private UI_QuestPopup _popup;
        private QuestTabType _questTabType = QuestTabType.DailyQuest;
        private QuestModel _model = ModelFactory.CreateOrGetModel<QuestModel>();
        
        public async void OpenPopup()
        {
            try
            {
                await GetQuestDataAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"error {e.Message}");
                ClosePopup();
                return;
            }

            _questTabType = QuestTabType.DailyQuest;
            _popup = Manager.I.UI.OpenPopup<UI_QuestPopup>();
            _popup.AddEvent(ClosePopup,
                () => OnChangeQuestTab(QuestTabType.DailyQuest),
                () => OnChangeQuestTab(QuestTabType.Achievement),
                OnClaimDailyQuest,
                OnClaimStageQuest,
                OnClearAchievement);
            _popup.UpdateUI(_questTabType, _model);
        }
        
        private void OnChangeQuestTab(QuestTabType questTabType)
        {
            if (_questTabType == questTabType)
            {
                return;
            }

            _questTabType = questTabType;
            _popup.UpdateUI(questTabType, _model);
        }

        private void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        private async void OnClaimDailyQuest(int questId)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            DailyQuestClaimRequestData requestData = new DailyQuestClaimRequestData();
            requestData.questMissionId = questId.ToString();
            var response = await Manager.I.Web.SendRequest<ClaimDailyQuestResponseData>("/quest/daily/quest/claim",
                requestData, MethodType.POST.ToString());

            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {   
                switch (response.message)
                {
                    case ServerMessageType.USER_QUEST_NOT_COMPLETED:
                        string message = Manager.I.Data.LocalizationDataDict["USER_QUEST_NOT_COMPLETED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                    case ServerMessageType.USER_QUEST_REWARD_ALREADY_CLAIMED:
                        message = Manager.I.Data.LocalizationDataDict["USER_QUEST_REWARD_ALREADY_CLAIMED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                }

                return;
            }
            
            QuestModel model = ModelFactory.CreateOrGetModel<QuestModel>();
            int questPoint = response.data.totalQuestPoint - model.totalQuestPoint;
            model.SetQuestData(response.data.quests, response.data.questStages, response.data.totalQuestPoint, null);
            _popup.UpdateUI(_questTabType, _model);
            var presenter = PresenterFactory.CreateOrGet<QuestRewardPopupPresenter>();
            presenter.OpenPopup(0, 0, 0, 0, 0, 0, 0, 0, questPoint);
        }

        private async void OnClaimStageQuest(int questId)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            DailyStageQuestClaimRequestData requestData = new DailyStageQuestClaimRequestData();
            requestData.questStageId = questId.ToString();
            var response = await Manager.I.Web.SendRequest<ClaimDailyQuestResponseData>("/quest/daily/stage/claim",
                requestData, MethodType.POST.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                switch (response.message)
                {
                    case ServerMessageType.USER_QUEST_STAGE_REWARD_ALREADY_CLAIMED:
                        string message = Manager.I.Data.LocalizationDataDict["USER_QUEST_NOT_COMPLETED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                    case ServerMessageType.USER_QUEST_STAGE_REWARD_NOT_ENOUGH_POINT:
                        message = Manager.I.Data.LocalizationDataDict["USER_QUEST_REWARD_ALREADY_CLAIMED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                }
                
                return;
            }

            QuestModel model = ModelFactory.CreateOrGetModel<QuestModel>();
            model.SetQuestData(response.data.quests, response.data.questStages, response.data.totalQuestPoint, null);
            _popup.UpdateUI(_questTabType, _model);

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Inventory.gold;
            int jewel = response.data.inventory.jewel - userModel.Inventory.jewel;
            int weaponScroll = response.data.inventory.weaponScroll - userModel.Inventory.weaponScroll;
            int glovesScroll = response.data.inventory.glovesScroll - userModel.Inventory.glovesScroll;
            int ringScroll = response.data.inventory.ringScroll - userModel.Inventory.ringScroll;
            int armorScroll = response.data.inventory.armorScroll - userModel.Inventory.armorScroll;
            int beltScroll = response.data.inventory.beltScroll - userModel.Inventory.beltScroll;
            int bootsScroll = response.data.inventory.bootsScroll - userModel.Inventory.bootsScroll;
            
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
            userModel.SetInventory(response.data.inventory);

            var presenter = PresenterFactory.CreateOrGet<QuestRewardPopupPresenter>();
            presenter.OpenPopup(gold, jewel, weaponScroll, glovesScroll, ringScroll, armorScroll, beltScroll,
                bootsScroll, 0);
        }

        public async UniTask GetQuestDataAsync()
        {
            var response =
                await Manager.I.Web.SendRequest<DailyQuestResponseData>("/quest", null,
                    MethodType.GET.ToString());
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                return;
            }

            QuestModel model = ModelFactory.CreateOrGetModel<QuestModel>();
            model.SetQuestData(response.data.quests, response.data.questStages, response.data.totalQuestPoint, response.data.achievements);
        }

        public async void OnClearAchievement(int achievementId)
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<GetAchievementResponseData>($"/achievement/{achievementId}/clear", null,
                    MethodType.PATCH.ToString());

            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                switch (response.message)
                {
                    case ServerMessageType.ACHIEVEMENT_NOT_FOUND:
                        string message = Manager.I.Data.LocalizationDataDict["ACHIEVEMENT_NOT_FOUND"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                    case ServerMessageType.ACHIEVEMENT_NOT_CLEARED:
                        message = Manager.I.Data.LocalizationDataDict["ACHIEVEMENT_NOT_CLEARED"].GetValueByLanguage();
                        Manager.I.UI.OpenSystemPopup(message);
                        break;
                }
                return;
            }

            Inventory inventory = response.data.inventory;
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int gold = response.data.inventory.gold - userModel.Inventory.gold;
            int jewel = response.data.inventory.jewel - userModel.Inventory.jewel;
            int weaponScroll = response.data.inventory.weaponScroll - userModel.Inventory.weaponScroll;
            int glovesScroll = response.data.inventory.glovesScroll - userModel.Inventory.glovesScroll;
            int ringScroll = response.data.inventory.ringScroll - userModel.Inventory.ringScroll;
            int armorScroll = response.data.inventory.armorScroll - userModel.Inventory.armorScroll;
            int beltScroll = response.data.inventory.beltScroll - userModel.Inventory.beltScroll;
            int bootsScroll = response.data.inventory.bootsScroll - userModel.Inventory.bootsScroll;
          
            if (jewel > 0 || gold > 0)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.GetGold);
            }
            userModel.SetInventory(response.data.inventory);

            QuestModel questModel = ModelFactory.CreateOrGetModel<QuestModel>();
            questModel.SetAchievementData(response.data.achievements);
            _popup.UpdateUI(_questTabType, _model);

            var presenter = PresenterFactory.CreateOrGet<QuestRewardPopupPresenter>();
            presenter.OpenPopup(gold, jewel, weaponScroll, glovesScroll, ringScroll, armorScroll, beltScroll,
                bootsScroll, 0);
        }
    }
}