using System;
using MewVivor.Common;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Presenter;
using MewVivor.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MewVivor.Popup
{
    public class UI_BattlePopup : BasePopup
    {
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _stageSelectButton;
        
        [SerializeField] private Button _vipShopButton;
        [SerializeField] private Button _questBoxButton;
        [SerializeField] private Button _huntPassButton;
        [SerializeField] private Button _huntPassAdsButton;
        [SerializeField] private TextMeshProUGUI _huntPassRemainAdsValueText;
        [SerializeField] private GameObject _huntPassInfoObject;

        [SerializeField] private TextMeshProUGUI _stageLevelText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        [SerializeField] private TextMeshProUGUI _staminaAmountText;

        [SerializeField] private GameObject _huntPassAdsObject;
        [SerializeField] private GameObject _huntPassAppliedObject;
        [SerializeField] private TextMeshProUGUI _remainBoostText;
        
        [SerializeField] private GameObject _vipReddotObject;
        [SerializeField] private GameObject _questReddotObject;
        [SerializeField] private GameObject _huntPassReddotObject;
        [SerializeField] private GameObject _mailReddotObject;
        [SerializeField] private LobbyTopMenuButton _lobbyTopMenuButton;

        private CompositeDisposable _subscriptionDisposables;

        public void AddEvent(Action onGameStartAction, Action onOpenStageSelectPopupAction,
            Action onOpenQuestPopupAction, Action onShowHuntPassAdsAction, Action onShowHuntPassPopupAction, Action onShowVipPopupAction)
        {
            _gameStartButton.SafeAddButtonListener(onGameStartAction.Invoke);
            _stageSelectButton.SafeAddButtonListener(onOpenStageSelectPopupAction.Invoke);
            _questBoxButton.SafeAddButtonListener(onOpenQuestPopupAction.Invoke);
            _huntPassAdsButton.SafeAddButtonListener(onShowHuntPassAdsAction.Invoke);
            _huntPassButton.SafeAddButtonListener(onShowHuntPassPopupAction.Invoke);
            _vipShopButton.SafeAddButtonListener(onShowVipPopupAction.Invoke);

            if (_subscriptionDisposables == null)
            {
                _subscriptionDisposables = new CompositeDisposable();
                var shopModel = ModelFactory.CreateOrGetModel<ShopModel>();
                shopModel.IsPossiblePurchaseVipAndVVip.Subscribe(x => { _vipReddotObject.SetActive(x); })
                    .AddTo(_subscriptionDisposables);

                var questModel = ModelFactory.CreateOrGetModel<QuestModel>();
                questModel.IsPossibleGetRewardQuestOrAchievement.Subscribe(x => { _questReddotObject.SetActive(x); })
                    .AddTo(_subscriptionDisposables);

                var huntPassModel = ModelFactory.CreateOrGetModel<HuntPassModel>();
                huntPassModel.IsPossibleGetReward.Subscribe(x => { _huntPassReddotObject.SetActive(x); })
                    .AddTo(_subscriptionDisposables);

                var userModel = ModelFactory.CreateOrGetModel<UserModel>();
                userModel.IsPossibleGetMail.Subscribe(x => { _mailReddotObject.SetActive(x); })
                    .AddTo(_subscriptionDisposables);
            }
        }

        private void OnDestroy()
        {
            _subscriptionDisposables?.Dispose();
        }

        private void OnEnable()
        {
            Manager.I.Event?.AddEvent(GameEventType.ChangeLanguage, OnChangeLanguage);   
        }

        protected override void OnDisable()
        {
            Manager.I.Event?.RemoveEvent(GameEventType.ChangeLanguage, OnChangeLanguage);
        }

        private void OnChangeLanguage(object value)
        {
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            StageHistoryData stageHistoryData = userModel.GetStageHistory(Manager.I.SelectedStageIndex);
            string stageLevel = $"{stageHistoryData?.stage ?? Manager.I.SelectedStageIndex}";
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(stageHistoryData?.survivalTime ?? 0);
            string formatted = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            string stageBestScore = formatted;
            _stageLevelText.text = Manager.I.LanguageType == LanguageType.Eng ? $"Stage.{stageLevel}" : $"스테이지.{stageLevel}";
            _bestScoreText.text =  Manager.I.LanguageType == LanguageType.Eng ? $"Best time : {stageBestScore}" : $"최고 기록 : {stageBestScore}";
        }

        public void UpdateUI(string stageLevel, string stageBestScore, string staminaAmount,
            bool isPossibleShowHuntPassAds, string remainAdsCountValue, string remainBoostCount, bool isPossibleUseBoost)
        {
            _stageLevelText.text = Manager.I.LanguageType == LanguageType.Eng ? $"Stage.{stageLevel}" : $"스테이지.{stageLevel}";
            _bestScoreText.text =  Manager.I.LanguageType == LanguageType.Eng ? $"Best time : {stageBestScore}" : $"최고 기록 : {stageBestScore}";
            _staminaAmountText.text = staminaAmount;

            _huntPassInfoObject.SetActive(isPossibleShowHuntPassAds);
            _huntPassAdsButton.gameObject.SetActive(isPossibleShowHuntPassAds);
            _huntPassRemainAdsValueText.text = remainAdsCountValue;
            _remainBoostText.text = remainBoostCount;
            
            _huntPassAdsObject.SetActive(!isPossibleUseBoost);
            _huntPassAppliedObject.SetActive(isPossibleUseBoost);
        }

        public void CloseBGPanel()
        {
            _lobbyTopMenuButton.CloseBGPanel();
        }
    }
}