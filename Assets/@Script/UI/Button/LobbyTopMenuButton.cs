using System;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Presenter;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MewVivor.UI
{
    public class LobbyTopMenuButton : UI_BaseButton
    {
        [SerializeField] private GameObject _toggleObject;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _mailButton;
        [SerializeField] private GameObject _mailRedDotObject;
        [SerializeField] private GameObject _bgButton;

        private bool _isActivateToggle;

        protected override void Start()
        {
            base.Start();
            _isActivateToggle = false;

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.IsPossibleGetMail.Subscribe(x =>
            {
                _mailRedDotObject.SetActive(x);
            }).AddTo(this);
        }

        private void OnDisable()
        {
            _isActivateToggle = false;
            _toggleObject.SetActive(false);
            CloseBGPanel();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            _button.SafeAddButtonListener(OnClickToggle);
            _settingButton.SafeAddButtonListener(()=> OpenPopup(true));
            _mailButton.SafeAddButtonListener(()=> OpenPopup(false));
        }

        private void OpenPopup(bool isSettingPopup)
        {
            var presenter = PresenterFactory.CreateOrGet<LobbyTopMenuPresenter>();
            presenter.OpenPopup(isSettingPopup);
        }

        public void OnClickToggle()
        {
            _isActivateToggle = !_isActivateToggle;
            _toggleObject.SetActive(_isActivateToggle);
            _bgButton.SetActive(_isActivateToggle);
        }

        public void CloseBGPanel()
        {
            _isActivateToggle = false;
            _toggleObject.SetActive(false);
            _bgButton.SetActive(false);
        }
    }
}