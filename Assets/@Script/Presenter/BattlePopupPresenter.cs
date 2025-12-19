using System;
using System.Globalization;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class BattlePopupPresenter : BasePresenter
    {
        private UI_BattlePopup _popup;
        
        public void Initialize()
        {
        }

        public void OpenPopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_BattlePopup>();
            _popup.AddEvent(OnGameStart, OnOpenStageSelectPopup, OnOpenQuestPopup, OnShowHuntPassAdsPopup,
                OnShowHuntPassPopup, OnShowVipPopup);
            Refresh();
        }

        public void Refresh()
        {
            HuntPassConfigData wavePassAdsCountData =
                Manager.I.Data.HuntPassConfigDataDict[HuntPassConfigName.WavePassAdsCount];
            HuntPassConfigData boostCountData =
                Manager.I.Data.HuntPassConfigDataDict[HuntPassConfigName.WavePassBoostCount];
            var passModel = ModelFactory.CreateOrGetModel<HuntPassModel>();
            int totalCount = int.Parse(wavePassAdsCountData.Value);
            bool isPossibleShowHuntPassAds = passModel.DailyBoostChargeCount < 2;
            string remainDailyBoostChargeCount = $"{totalCount - passModel.DailyBoostChargeCount}/{totalCount}";
            
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            StageHistoryData stageHistoryData = userModel.GetStageHistory(Manager.I.SelectedStageIndex);
            string stageLevel = $"{stageHistoryData?.stage ?? Manager.I.SelectedStageIndex}";
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(stageHistoryData?.survivalTime ?? 0);
            string formatted = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            string stageBestScore = formatted;
            string staminaAmount = Manager.I.Data.GlobalConfigDataDict[GlobalConfigName.StageEnterStamina].Value
                .ToString(CultureInfo.InvariantCulture);
            string remainBoostCount = $"{passModel.RemainBoostCount}/{boostCountData.Value}";
            bool isPossibleUseBoost = passModel.RemainBoostCount > 0;
            _popup.UpdateUI(stageLevel, stageBestScore, staminaAmount, isPossibleShowHuntPassAds,
                remainDailyBoostChargeCount, remainBoostCount, isPossibleUseBoost);
        }

        public void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        public void CloseBGPanel()
        {
            _popup.CloseBGPanel();
        }

        private async void OnGameStart()
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            float staminaAmount = Manager.I.Data.GlobalConfigDataDict[GlobalConfigName.StageEnterStamina].Value;
            if (userModel.Stamina.Value < staminaAmount)
            {
                string message = Manager.I.Data.LocalizationDataDict["Not_enough_stamina"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }

            await Manager.I.StartGame(GameType.MAIN, Manager.I.SelectedStageIndex);
        }
        
        private void OnOpenStageSelectPopup()
        {
            var presenter = PresenterFactory.CreateOrGet<StageSelectPopupPresenter>();
            presenter.OpenStageSelectPopup();
        }

        private void OnOpenQuestPopup()
        {
            var presenter = PresenterFactory.CreateOrGet<QuestPopupPresenter>();
            presenter.OpenPopup();
        }

        private async void OnShowHuntPassAdsPopup()
        {
            Manager.I.Ads.ShowRewardAd(OnRequestHuntPassBoost, OnFailedShowAds);
        }

        private void OnFailedShowAds()
        {
            string message = Manager.I.Data.LocalizationDataDict["Failed_ads"].GetValueByLanguage();
            Manager.I.UI.OpenSystemPopup(message);
        }

        private async void OnRequestHuntPassBoost()
        {
            HuntPassConfigData wavePassAdsCountData =
                Manager.I.Data.HuntPassConfigDataDict[HuntPassConfigName.WavePassAdsCount];
            var passModel = ModelFactory.CreateOrGetModel<HuntPassModel>();
            int totalCount = int.Parse(wavePassAdsCountData.Value);
            bool isPossibleShowHuntPassAds = passModel.DailyBoostChargeCount < 2;
            if (!isPossibleShowHuntPassAds)
            {
                Debug.LogError($"Failed error {passModel.DailyBoostChargeCount} / {totalCount}");
                return;
            }
         
            var presenter = PresenterFactory.CreateOrGet<HuntPassPopupPresenter>();
            await presenter.RequestHuntPassBoostAsync();

            Refresh();
        }

        private void OnShowHuntPassPopup()
        {
            var presenter = PresenterFactory.CreateOrGet<HuntPassPopupPresenter>();
            presenter.OpenPopup();            
        }

        private void OnShowVipPopup()
        {
            var presenter = PresenterFactory.CreateOrGet<VipShopPopupPresenter>();
            presenter.OpenPopup();
        }
    }
}