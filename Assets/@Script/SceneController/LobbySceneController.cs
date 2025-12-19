using System;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Presenter;
using MewVivor.UI;
using UnityEngine;

namespace MewVivor.Controller
{
    public class LobbySceneController : BaseSceneController
    {
        [SerializeField] private GameObject _prefab;
        private void Start()
        {
            Manager manager = Manager.I;
            var lobbyUI = manager.UI.ShowUI<UI_LobbyScene>();
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            lobbyPresenter.Initialize(userModel, lobbyUI);

            var battlePopupPresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePopupPresenter.OpenPopup();
        }

        private async void Update()
        {
            
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetUserResponseDataData response =
                    await Manager.I.Web.SendRequest<GetUserResponseDataData>("/user", new AuthRequestData(),
                        MethodType.GET.ToString());
                
                UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
                userModel.SetUserData(response.data);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Instantiate(_prefab);
            }
            
            #endif
            // if (Input.GetKeyDown(KeyCode.Alpha2))
            // {
            //     var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            //     Vector3 endPoint = lobbyPresenter.JewelImagePosition;
            //     CurrencyCollectAction.I.ShowAnimation(CurrencyType.Jewel, 10, endPoint);
            // }
        }
    }
}