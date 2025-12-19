using System;
using System.Collections.Generic;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Popup;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class GameResultPopupPresenter : BasePresenter
    {
        private GameEndType _gameEndType;
        public async void OpenGameResultPopup(int playTime, string stageResult, GameType gameType)
        {
            TimeScaleHandler.RequestPause();
            int aliveTime = 0;
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            if (gameType == GameType.MAIN)
            {
                _gameEndType = Manager.I.Object.Player.IsDead ? GameEndType.LOSE : GameEndType.CLEAR;
                Debug.Log($"stage {Manager.I.Game.StageLevel}");
                aliveTime = userModel.GetStageBestAliveTime(Manager.I.Game.StageLevel);
            }
            else
            {
                _gameEndType = GameEndType.CLEAR;
                aliveTime = userModel.GetInfiniteBestAliveTime();
            }
            
            TimeSpan playTimeSpan = TimeSpan.FromSeconds(playTime);
            TimeSpan aliveTimeSpan = TimeSpan.FromMilliseconds(aliveTime);
            string playTimeValue = $"{playTimeSpan.Minutes:D2}:{playTimeSpan.Seconds:D2}";
            string bestSurvivalTime = playTimeSpan.Milliseconds > aliveTimeSpan.Milliseconds
                ? playTimeValue
                : $"{aliveTimeSpan.Minutes:D2}:{aliveTimeSpan.Seconds:D2}";
            bool isNewRecord = playTimeSpan.Milliseconds > aliveTimeSpan.Milliseconds;
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            Manager.I.Audio.Play(Sound.SFX, SoundKey.StageClear);
            var rewardItemDict = playerModel.GetAcquiredRewardItemDict();
            int killCount = stageModel.KillCount.Value;
            var gameResultPopup = Manager.I.UI.OpenPopup<UI_GameResultPopup>();
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
            await Manager.I.Game.RequestGameEnd(_gameEndType);
            Manager.I.Game.GameEnd();
        }
    }
}