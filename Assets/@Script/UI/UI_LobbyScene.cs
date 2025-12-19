using System;
using System.Linq;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Interface;
using MewVivor.Model;
using MewVivor.Presenter;
using MewVivor.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MewVivor.UI
{
    public class UI_LobbyScene : BaseUI, IView
    {
        public Vector3 GoldImagePosition => _goldImageTransform.position;
        public Vector3 JewelImagePosition => _jewelImageTransform.position;
        
        [SerializeField] private Animator _bottomTapAnimator;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _equipmentButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private Button _challengeButton;
        [SerializeField] private Button _evolutionButton;
        [SerializeField] private Button _goldButton;
        [SerializeField] private Button _jewelButton;

        [SerializeField] private TextMeshProUGUI _userNameText;
        [SerializeField] private TextMeshProUGUI _userLevelText;
        [SerializeField] private Image _userLevelImage;
     
        [SerializeField] private TextMeshProUGUI _staminaAmountText;
        [SerializeField] private TextMeshProUGUI _goldAmountText;
        [SerializeField] private TextMeshProUGUI _jewelAmounText;
        [SerializeField] private Button _profileButton;

        [SerializeField] private Transform _goldImageTransform;
        [SerializeField] private Transform _jewelImageTransform;

        [SerializeField] private GameObject _shopRedDotObject;
        [SerializeField] private GameObject _equipmentRedDotObject;
        [SerializeField] private GameObject _battleRedDotObject;
        [SerializeField] private GameObject _challengeRedDotObject;
        [SerializeField] private GameObject _evolutionRedDotObject;
    
        private static class AnimationName
        {
            public static int Shop = Animator.StringToHash("Shop");
            public static int Evolution = Animator.StringToHash("Evolution");
            public static int Equipment = Animator.StringToHash("Equipment");
            public static int Challenge = Animator.StringToHash("Challenge");
            public static int Battle = Animator.StringToHash("Battle");
        }
        
        public void AddEvent(Action<MenuToggleType> onClickMenuToggleAction, Action onShowProfilePopupAction, Action<float> onGotoShopPageAction)
        {
            _shopButton.SafeAddButtonListener(()=>
            {
                OnClickMenuToggle(MenuToggleType.Shop, onClickMenuToggleAction);
            });
            
            _equipmentButton.SafeAddButtonListener(()=>
            {
                OnClickMenuToggle(MenuToggleType.Equipment, onClickMenuToggleAction);
            });   
            
            _battleButton.SafeAddButtonListener(()=>
            {
                OnClickMenuToggle(MenuToggleType.Battle, onClickMenuToggleAction);
            });
            
            _challengeButton.SafeAddButtonListener(()=>
            {
                OnClickMenuToggle(MenuToggleType.Challenge, onClickMenuToggleAction);
            });
            
            _evolutionButton.SafeAddButtonListener(()=>
            {
                OnClickMenuToggle(MenuToggleType.Evolution, onClickMenuToggleAction);
            });

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.Stamina.Subscribe(x =>
            {
                _staminaAmountText.text = x.ToString();
            }).AddTo(this);

            userModel.Gold.Subscribe(x =>
            {
                _goldAmountText.text = x.ToString();
            }).AddTo(this);

            userModel.Jewel.Subscribe(x =>
            {
                _jewelAmounText.text = x.ToString();
            }).AddTo(this);

            var shopModel = ModelFactory.CreateOrGetModel<ShopModel>();
            shopModel.IsExistFreeReward.Subscribe(x =>
            {
                _shopRedDotObject.SetActive(x);
            }).AddTo(this);

            userModel.InfinityTicket.Subscribe(x =>
            {
                _challengeRedDotObject.SetActive(x > 0);
            }).AddTo(this);

            userModel.Gold.Subscribe(x =>
            {
                EvolutionPopupPresenter evolutionPopupPresenter =
                    PresenterFactory.CreateOrGet<EvolutionPopupPresenter>();
                int level = userModel.UserData.evolutionCount;
                float price = evolutionPopupPresenter.CalculatePrice(level);
                _evolutionRedDotObject.SetActive(x >= price);
            }).AddTo(this);

            userModel.IsPossibleMerge.Subscribe(x => { _equipmentRedDotObject.SetActive(x); })
                .AddTo(this);
            var questModel = ModelFactory.CreateOrGetModel<QuestModel>();
            var huntPassModel = ModelFactory.CreateOrGetModel<HuntPassModel>();
            
            Observable.CombineLatest(
                    shopModel.IsPossiblePurchaseVipAndVVip,
                    questModel.IsPossibleGetRewardQuestOrAchievement,
                    huntPassModel.IsPossibleGetReward,
                    userModel.IsPossibleGetMail
                )
                .Select(list => list.All(x => x ==false))
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    _battleRedDotObject.SetActive(!x);
                }).AddTo(this);
            
            _goldButton.SafeAddButtonListener(()=> onGotoShopPageAction.Invoke(0.48f));
            _jewelButton.SafeAddButtonListener(() => onGotoShopPageAction.Invoke(0.35f));
            _profileButton.SafeAddButtonListener(onShowProfilePopupAction.Invoke);
        }

        public void UpdateUI(string userName, string userLevel, float userLevelRatioValue)
        {
            _userNameText.text = userName;
            _userLevelText.text = userLevel;
            _userLevelImage.fillAmount = userLevelRatioValue;
        }

        public void PlayBottomTapAnimation(MenuToggleType menuToggleType)
        {
             int hash = GetAnimationHash(menuToggleType);
             _bottomTapAnimator.Play(hash);
        }

        private void OnClickMenuToggle(MenuToggleType menuToggleType, Action<MenuToggleType> onClickMenuToggleAction)
        {
            PlayBottomTapAnimation(menuToggleType);
            onClickMenuToggleAction?.Invoke(menuToggleType);
        }

        private int GetAnimationHash(MenuToggleType menuToggleType)
        {
            switch (menuToggleType)
            {
                case MenuToggleType.Battle:
                    return AnimationName.Battle;
                case MenuToggleType.Equipment:
                    return AnimationName.Equipment;
                case MenuToggleType.Shop:
                    return AnimationName.Shop;
                case MenuToggleType.Challenge:
                    return AnimationName.Challenge;
                case MenuToggleType.Evolution:
                    return AnimationName.Evolution;
            }

            Debug.LogError("Failed get hash : " + menuToggleType);
            return -1;
        }
    }
}