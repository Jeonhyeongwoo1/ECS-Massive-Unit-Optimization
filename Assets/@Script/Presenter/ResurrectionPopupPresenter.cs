using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Stage;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class ResurrectionPopupPresenter : BasePresenter
    {
        private UI_ResurrectionPopup _popup;
        private CancellationTokenSource _timerCts;

        public void OpenResurrectionPopup()
        {
            Debug.Log("Open Resurrection Popup");
            TimeScaleHandler.RequestPause();
            _popup = Manager.I.UI.OpenPopup<UI_ResurrectionPopup>();
            _popup.AddEvent(OnShowAds, OnContinueGame, OnGameDone);

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            int jewel = userModel.Jewel.Value;
            GlobalConfigData data = Manager.I.Data.GlobalConfigDataDict[GlobalConfigName.ReviveCostJewel];
            _popup.UpdateUI(data.Value.ToString(CultureInfo.InvariantCulture), jewel >= data.Value);
            _timerCts = new CancellationTokenSource();
            StartTimer().Forget();
        }

        private async UniTask StartTimer()
        {
            int interval = 1;
            int remainTime = Const.ResurrectionRemainTime;
            while (remainTime > 0)
            {
                remainTime -= interval;
                _popup.UpdateTimer(remainTime.ToString());
                try
                {
                    await UniTask.Delay(interval * 1000, DelayType.UnscaledDeltaTime,
                        cancellationToken: _timerCts.Token);
                }
                catch (Exception e) when(e is not OperationCanceledException)
                {
                    Debug.LogError($"error {e.Message}");
                    return;
                }
            }
            
            OnGameDone();
        }

        private void OnGameDone()
        {
            if (_popup != null)
            {
                Manager.I.UI.ClosePopup();
            }

            TimeScaleHandler.ReleasePause();
            Utils.SafeCancelCancellationTokenSource(ref _timerCts);
            GameManager gameManager = Manager.I.Game;
            StageBase currentStage = gameManager.CurrentStage;
            GameType gameType = gameManager.GameType;
            string stageResult = gameType == GameType.INFINITY
                ? "InfinityMode"
                : (currentStage as NormalStage)?.StageLevel.ToString();
            var presenter = PresenterFactory.CreateOrGet<GameOverPopupPresenter>();
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            presenter.OpenGameOverPopup(stageModel.ElapsedGameTime.Value, stageResult, gameType);
        }

        private void OnContinueGame()
        {
            UseResurrection(RetryType.JEWEL);
        }

        private async void UseResurrection(RetryType retryType)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OnGameDone();
                return;
            }
            
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            GameManager gameManager = Manager.I.Game;
            StageBase currentStage = gameManager.CurrentStage;
            TimeSpan playTimeSpan = DateTime.UtcNow - currentStage.GameStartTime;
            GameRetryRequestData gameEndRequestData = new GameRetryRequestData
            {
                gameSessionId = playerModel.GameSessionId,
                clearedWave = stageModel.CurrentWaveStep.Value,
                normalMonsterKillCount = stageModel.NormalMonsterKillCount.Value,
                eliteMonsterKillCount = stageModel.EliteMonsterKillCount.Value,
                bossMonsterKillCount = stageModel.BossMonsterKillCount.Value,
                survivalTime = playTimeSpan.Milliseconds,
                dropItems = new Dictionary<string, int>()
                {
                    { Const.ID_GOLD.ToString(), playerModel.Gold.Value },
                }
            };

            var response = await Manager.I.Web.SendRequest<GameRetryResponseData>($"/user-game/retry/{retryType}",
                gameEndRequestData,
                MethodType.POST.ToString());

            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                OnGameDone();
                return;
            }
            
            Utils.SafeCancelCancellationTokenSource(ref _timerCts);
            playerModel.SetGameSessionId(response.data.gameSessionId);
            TimeScaleHandler.ReleasePause();
            Manager.I.Event.Raise(GameEventType.UseResurrection, false);
            _popup.ClosePopup();
        }

        private void OnShowAds()
        {
            Utils.SafeCancelCancellationTokenSource(ref _timerCts);
            Manager.I.Ads.ShowRewardAd(() =>
            {
                UI_BlockCanvas.I.ShowAndHideBlockCanvas(true);
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    UI_BlockCanvas.I.ShowAndHideBlockCanvas(false);
                    // Time.timeScale = 0;
                });
                UseResurrection(RetryType.ADVERTISEMENT);
            }, OnContinueGame);
        }
    }
}