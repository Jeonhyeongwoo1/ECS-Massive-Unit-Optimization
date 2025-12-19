using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_SettingPopup : BasePopup
    {
        [Serializable]
        public struct ButtonGroup
        {
            public SettingType settingType;
            public Button button;
            public GameObject onObject;
        }

        [Serializable]
        public struct LanguageButtonGroup
        {
            public LanguageType languageType;
            public Button button;
            public GameObject selectObject;
        }

        [SerializeField] private Button _closeButton;
        [SerializeField] private List<ButtonGroup> _buttonGroupList;
        [SerializeField] private List<LanguageButtonGroup> _languageButtonGroupList;
        [SerializeField] private TextMeshProUGUI _uidText;
        [SerializeField] private Button _copyButton;
        [SerializeField] private Button _logoutButton;
        
        public void AddEvent(Action onCloseAction, Action onCopyAction, Action<LanguageType> onChangeLanguageAction, Action onLogoutAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _copyButton.SafeAddButtonListener(onCopyAction.Invoke);
            _logoutButton.SafeAddButtonListener(onLogoutAction.Invoke);
            _bgCloseButton.SafeAddButtonListener(onCloseAction.Invoke);
            
            foreach (LanguageButtonGroup group in _languageButtonGroupList)
            {
                group.button.SafeAddButtonListener(() =>
                {
                    foreach (LanguageButtonGroup languageButtonGroup in _languageButtonGroupList)
                    {
                        languageButtonGroup.selectObject.SetActive(languageButtonGroup.button == group.button);
                    }

                    onChangeLanguageAction.Invoke(group.languageType);
                });
            }
        }
        
        public void UpdateUI(List<SettingData> settingDataList, string uid)
        {
            foreach (LanguageButtonGroup group in _languageButtonGroupList)
            {
                group.selectObject.SetActive(Manager.I.LanguageType == group.languageType);
            }
            
            foreach (SettingData settingData in settingDataList)
            {
                ButtonGroup group = _buttonGroupList.Find(v => v.settingType == settingData.settingType);
                group.onObject.SetActive(!settingData.isOn);
                group.button.SafeAddButtonListener(
                    () => settingData.onActivateAction.Invoke(group.settingType));
            }

            _uidText.text = uid;
        }
    }
}