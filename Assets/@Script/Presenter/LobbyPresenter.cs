using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.UI;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class LobbyPresenter : BasePresenter
    {
        public Vector3 GoldImagePosition => _lobbySceneView.GoldImagePosition;
        public Vector3 JewelImagePosition => _lobbySceneView.JewelImagePosition;
        
        private UI_LobbyScene _lobbySceneView;
        private UserModel _model;
        private MenuToggleType _menuToggleType = MenuToggleType.None;
        
        public void Initialize(UserModel model, UI_LobbyScene view)
        {
            _lobbySceneView = view;
            _model = model;
            UpdateUI();
            _menuToggleType = MenuToggleType.Battle;
        }

        public void UpdateUI()
        {
            long accumulatedExp = _model.AccumulatedExp.Value;
            (int, float) data = Utils.GetCurrentLevelAndRemainExpRatio(accumulatedExp);
            _lobbySceneView.AddEvent(OnClickMenuToggle, OnShowProfilePopup, GotoShopPage);
            _lobbySceneView.UpdateUI(_model.UserData.name, data.Item1.ToString(), data.Item2);
        }

        private void GotoShopPage(float ratio)
        {
            _lobbySceneView.PlayBottomTapAnimation(MenuToggleType.Shop);
            OnClickMenuToggle(MenuToggleType.Shop);
            var shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
            shopPopupPresenter.SetVerticalNormalizedPosition(ratio);
        }
        
        private void OnShowProfilePopup()
        {
            var profilePopupPresenter = PresenterFactory.CreateOrGet<ProfilePopupPresenter>();
            profilePopupPresenter.OpenPopup();
        }
        
        private async void OnClickMenuToggle(MenuToggleType menuToggleType)
        {
            if (_menuToggleType == menuToggleType)
            {
                return;
            }
            
            switch (_menuToggleType)
            {
                case MenuToggleType.None:
                    break;
                case MenuToggleType.Battle:
                    BattlePopupPresenter battlePopupPresenter =
                        PresenterFactory.CreateOrGet<BattlePopupPresenter>();
                    battlePopupPresenter.ClosePopup();
                    break;
                case MenuToggleType.Equipment:
                    EquipmentPopupPresenter equipmentPopupPresenter =
                        PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
                    equipmentPopupPresenter.ClosePopup();
                    break;
                case MenuToggleType.Shop:
                    ShopPopupPresenter shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
                    shopPopupPresenter.ClosePopup();
                    break;
                case MenuToggleType.Challenge:
                    ChallengePopupPresenter challengePopupPresenter =
                        PresenterFactory.CreateOrGet<ChallengePopupPresenter>();
                    challengePopupPresenter.ClosePopup();
                    break;
                case MenuToggleType.Evolution:
                    EvolutionPopupPresenter evolutionPopupPresenter =
                        PresenterFactory.CreateOrGet<EvolutionPopupPresenter>();
                    evolutionPopupPresenter.ClosePopup();
                    break;
            }
            
            _menuToggleType = menuToggleType;

            switch (_menuToggleType)
            {
                case MenuToggleType.None:
                    break;
                case MenuToggleType.Battle:
                    BattlePopupPresenter battlePopupPresenter =
                        PresenterFactory.CreateOrGet<BattlePopupPresenter>();
                    battlePopupPresenter.Initialize();
                    battlePopupPresenter.OpenPopup();
                    break;
                case MenuToggleType.Equipment:
                    EquipmentPopupPresenter equipmentPopupPresenter =
                        PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
                    equipmentPopupPresenter.Initialize();
                    equipmentPopupPresenter.OpenPopup();
                    break;
                case MenuToggleType.Shop:
                    ShopPopupPresenter shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
                    shopPopupPresenter.OpenPopup();
                    break;
                case MenuToggleType.Challenge:
                    ChallengePopupPresenter challengePopupPresenter =
                        PresenterFactory.CreateOrGet<ChallengePopupPresenter>();
                    challengePopupPresenter.OpenPopup();
                    break;
                case MenuToggleType.Evolution:
                    EvolutionPopupPresenter evolutionPopupPresenter =
                        PresenterFactory.CreateOrGet<EvolutionPopupPresenter>();
                    evolutionPopupPresenter.OpenPopup();
                    break;
            }
        }
    }
}