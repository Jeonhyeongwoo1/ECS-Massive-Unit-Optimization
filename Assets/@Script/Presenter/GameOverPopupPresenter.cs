using System;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class GameOverPopupPresenter : BasePresenter
    {
        private GameEndType _gameEndType;
        
        public void OpenGameOverPopup(int playTime, string stageResult, GameType gameType)
        {
            if (gameType == GameType.MAIN)
            {
                _gameEndType = Manager.I.Object.Player.IsDead ? GameEndType.LOSE : GameEndType.CLEAR;
            }
            else
            {
                _gameEndType = GameEndType.CLEAR;
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            // int aliveTime = userModel.GetStageBestAliveTime(Manager.I.Game.StageLevel);
            int aliveTime = 0;
            TimeSpan playTimeSpan = TimeSpan.FromSeconds(playTime);
            TimeSpan aliveTimeSpan = TimeSpan.FromMilliseconds(aliveTime);
            string playTimeValue = $"{playTimeSpan.Minutes:D2}:{playTimeSpan.Seconds:D2}";
            string bestSurvivalTime = playTimeSpan > aliveTimeSpan
                ? playTimeValue
                : $"{aliveTimeSpan.Minutes:D2}:{aliveTimeSpan.Seconds:D2}";
            bool isNewRecord = playTimeSpan > aliveTimeSpan;
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            TimeScaleHandler.RequestPause();
            var rewardItemDict = playerModel.GetAcquiredRewardItemDict();
            int killCount = stageModel.KillCount.Value;
            var gameResultPopup = Manager.I.UI.OpenPopup<UI_GameOverPopup>();
            gameResultPopup.AddEvent(OnMoveToLobby, OnShowStatistics);
            gameResultPopup.UpdateUI(stageResult,
                playTimeValue,
                killCount,
                bestSurvivalTime,
                isNewRecord,
                gameType,
                rewardItemDict);
        }
        
        private void OnShowStatistics()
        {
            var presenter = PresenterFactory.CreateOrGet<StatisticsPopupPresenter>();
            presenter.OpenStatisticsPopup();
        }

        private async void OnMoveToLobby()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                string message = Manager.I.Data.LocalizationDataDict["Conenct_Internet"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }
            
            TimeScaleHandler.ReleasePause();
            // await Manager.I.Game.RequestGameEnd(_gameEndType);
            Manager.I.Game.GameEnd();
        }
    }
}