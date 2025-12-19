using System;
using System.Collections.Generic;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Presenter
{
    [Serializable]
    public struct SettingData
    {
        public SettingType settingType;
        public bool isOn;
        public Action<SettingType> onActivateAction;
    }

    public class SettingPopupPresenter : BasePresenter
    {
        private UI_SettingPopup _popup;

        public void OpenPopup()
        {
            _popup = Manager.I.UI.OpenPopup<UI_SettingPopup>();
            RefreshUI();    
        }

        private void OnClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            var battlePopupPresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePopupPresenter.CloseBGPanel();
            Manager.I.UI.ClosePopup();
        }
        
        private void RefreshUI()
        {
            var settingDataList = new List<SettingData>();
            int length = System.Enum.GetNames(typeof(SettingType)).Length;
            for (int i = 0; i < length; i++)
            {
                SettingType settingType = (SettingType)i;
                SettingData settingData = new SettingData
                {
                    isOn = settingType switch
                    {
                        SettingType.BGM => Manager.I.IsOnBGM,
                        SettingType.SFX => Manager.I.IsOnSfx,
                        SettingType.Vibration => Manager.I.IsOnHaptic
                    },
                    settingType = settingType,
                    onActivateAction = OnActivateSettingByType
                };
                settingDataList.Add(settingData);
            }

            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            string userId = userModel.UserData.id;

            _popup.AddEvent(OnClosePopup, OnCopyUID, OnChangeLanguage, OnLogout);
            _popup.UpdateUI(settingDataList, userId);
        }

        private void OnLogout()
        {
            string loginType = PlayerPrefs.GetString("LastLoginType");
            if (loginType != LoginType.GUEST.ToString())
            {
                var userModel = ModelFactory.CreateOrGetModel<UserModel>();
                userModel.RefreshToken = null;
                userModel.AccessToken = null;
                PlayerPrefs.SetString("AccessToken", null);
                PlayerPrefs.SetString("RefreshToken", null);
            }

            Manager.I.ChangeTitleScene();
        }

        private void OnCopyUID()
        {
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            string userId = userModel.UserData.id;
            GUIUtility.systemCopyBuffer = userId;
        }

        private void OnActivateSettingByType(SettingType settingType)
        {
            switch (settingType)
            {
                case SettingType.BGM:
                    Manager.I.IsOnBGM = !Manager.I.IsOnBGM;
                    break;
                case SettingType.SFX:
                    Manager.I.IsOnSfx = !Manager.I.IsOnSfx;
                    break;
                case SettingType.Vibration:
                    Manager.I.IsOnHaptic = !Manager.I.IsOnHaptic;
                    break;
            }
            
            RefreshUI();
        }

        private void OnChangeLanguage(LanguageType languageType)
        {
            Manager.I.ChangeLanguage(languageType);
        }
    }
}