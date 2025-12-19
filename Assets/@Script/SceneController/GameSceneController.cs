using System;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Presenter;
using UnityEngine;

namespace MewVivor.Controller
{
    public class GameSceneController : BaseSceneController
    {
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            var pausePopupPresenter = PresenterFactory.CreateOrGet<PausePopupPresenter>();
            pausePopupPresenter.Initialize(playerModel);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
        }
    }
}