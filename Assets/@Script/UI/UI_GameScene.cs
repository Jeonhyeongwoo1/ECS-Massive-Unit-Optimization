using System;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Presenter;
using MewVivor.UISubItemElement;
using MewVivor.View;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace MewVivor.InGame.View
{
    public class UI_GameScene : BaseUI
    {
        [SerializeField] private Image _playerExpImage;
        [SerializeField] private Image _playerExpBgImage;
        [SerializeField] private TextMeshProUGUI _playerLvelText;
        [SerializeField] private UI_BossAndEliteMonsterStateInfo _bossBossAndEliteMonsterStateInfo;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private TextMeshProUGUI _elaspedTimeText;
        [SerializeField] private TextMeshProUGUI _goldAmountText;
        [SerializeField] private TextMeshProUGUI _killAmountText;
        [SerializeField] private UI_MonsterAlarmElement _monsterAlarmElement;
        [SerializeField] private GameObject _playerExpGroupObject;
        [SerializeField] private Image _whiteflashImage;
        [SerializeField] private Image _damageFlashImage;
        
        private Sequence _damageFlashSequence;

        public override void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }

            Manager.I.Event.AddEvent(GameEventType.SpawnedBoss, OnSpawnedBoss);
            Manager.I.Event.AddEvent(GameEventType.EndWave, OnWaveEnd);
            Manager.I.Object.Player.onHitReceived += OnShowDamageFlashImage; 

            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            
            playerModel.CurrentExpRatio
                .Subscribe(OnChangedPlayerExp)
                .AddTo(this);
            playerModel.CurrentLevel
                .Subscribe(OnChangedCurrentLevel)
                .AddTo(this);
            playerModel.Gold
                .Subscribe(OnGoldAmount)
                .AddTo(this);

            if (Manager.I.Game.GameType == GameType.INFINITY)
            {
                stageModel.ElapsedGameTime
                    .Subscribe(OnChangedStageTimer)
                    .AddTo(this);
            }
            else
            {
                stageModel.ElapsedGameTime
                    .Subscribe(OnChangedStageTimer)
                    .AddTo(this);
            }
            
            stageModel.KillCount
                .Subscribe(OnChangedKillCount)
                .AddTo(this);

            _bossBossAndEliteMonsterStateInfo.AddEvents();
            _pauseButton.SafeAddButtonListener(OnOpenPausePopup);
            IsInitialize = true;
        }

        private void OnGoldAmount(int amount)
        {
            _goldAmountText.text = amount.ToString();
        }
        
        private void OnChangedKillCount(int killCount)
        {
            _killAmountText.text = killCount.ToString();
        }

        private void OnChangedStageTimer(int time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            _elaspedTimeText.text = timeSpan.ToString(@"mm\:ss");
        }

        private void OnDestroy()
        {
            Manager.I.Event.RemoveEvent(GameEventType.SpawnedBoss, OnSpawnedBoss);
            Manager.I.Event.RemoveEvent(GameEventType.EndWave, OnWaveEnd);
        }

        private void OnOpenPausePopup()
        {
            var pausePopupPresenter = PresenterFactory.CreateOrGet<PausePopupPresenter>();
            pausePopupPresenter.OpenPausePopup();
        }
    
        private void OnSpawnedBoss(object value)
        {
            _monsterAlarmElement.Show();
        }

        private void OnWaveEnd(object value)
        {
            _monsterAlarmElement.Hide();
        }

        public void ShowMonsterInfo(MonsterType monsterType, string monsterName, float ratio)
        {
            if (monsterType == MonsterType.Boss)
            {
                _bossBossAndEliteMonsterStateInfo.gameObject.SetActive(true);
                _bossBossAndEliteMonsterStateInfo.UpdateMonsterInfo(monsterName, ratio);
                _playerExpGroupObject.SetActive(false);
                // _elaspedTimeText.gameObject.SetActive(false);
            }
        }

        public void HideMonsterInfo(MonsterType monsterType)
        {
            if (monsterType == MonsterType.Boss)
            {
                _bossBossAndEliteMonsterStateInfo.gameObject.SetActive(false);
                _playerExpGroupObject.SetActive(true);
                // _elaspedTimeText.gameObject.SetActive(true);
            }
        }

        private void OnChangedCurrentLevel(int value)
        {
            _playerLvelText.text = $"LV.{value}";
        }

        private void OnChangedPlayerExp(float value)
        {
            _playerExpBgImage.fillAmount = value;
            _playerExpImage.fillAmount = value;
        }

        public void ShowWhiteFlash()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_whiteflashImage.DOFade(1, 0.3f))
                .Append(_whiteflashImage.DOFade(0, 0.3f));
        }

        private void OnShowDamageFlashImage(int hp, int maxHP)
        {
            if (_damageFlashSequence != null)
            {
                return;
            }
            
            _damageFlashSequence?.Kill();
            _damageFlashSequence = DOTween.Sequence();
            _damageFlashSequence.Append(_damageFlashImage.DOFade(0.5f, 0.3f))
                .Append(_damageFlashImage.DOFade(0, 0.3f))
                .OnComplete(() => _damageFlashSequence = null);
        }
    }
}
