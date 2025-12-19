using System;
using MewVivor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_ProfilePopup : BasePopup
    {
        [SerializeField] private Button _profilePopupcloseButton;
        [SerializeField] private Button _editProfilePopupButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private TextMeshProUGUI _nickNameText;
        [SerializeField] private TMP_InputField  _nickNameInputField;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private GameObject _editProfilePopupObject;
        [SerializeField] private TextMeshProUGUI _jewelPriceText;
        [SerializeField] private GameObject _jewelPricePanelObject;
        [SerializeField] private GameObject _confirmTextObject;
        [SerializeField] private GameObject _nicknameFreeChangeDescriptionObject;
        
        private string _inputFieldValue;
        
        public void AddEvent(Action onCloseAction, Action<string> onConfirm)
        {
            _profilePopupcloseButton.SafeAddButtonListener(onCloseAction.Invoke);
            _editButton.SafeAddButtonListener(OnClickEditButton);
            _editProfilePopupButton.SafeAddButtonListener(OnClickEditProfilePopupButton);
            _confirmButton.SafeAddButtonListener(() => onConfirm.Invoke(_inputFieldValue));
            _nickNameInputField.onValueChanged.RemoveAllListeners();
            _nickNameInputField.onValueChanged.AddListener(OnInputChanged);
        }

        private void OnClickEditProfilePopupButton()
        {
            _editProfilePopupObject.SetActive(false);
        }

        private void OnClickEditButton()
        {
            _editProfilePopupObject.SetActive(true);
        }

        public void UpdateUI(string nickname, string jewelPrice, bool isPossibleChangeNickNameByJewel, bool isFree)
        {
            _nickNameText.text = nickname;
            _nickNameInputField.text = nickname;
            _jewelPriceText.text = jewelPrice;
            _jewelPriceText.color = isPossibleChangeNickNameByJewel ? Color.white : Color.red;
            _confirmButton.interactable = isFree || isPossibleChangeNickNameByJewel;
            _jewelPricePanelObject.SetActive(!isFree);
            _confirmTextObject.SetActive(isFree);
            _editProfilePopupObject.SetActive(false);
            _nicknameFreeChangeDescriptionObject.SetActive(isFree);
        }

        private void OnInputChanged(string input)
        {
            _inputFieldValue = input;
        }
    }
}