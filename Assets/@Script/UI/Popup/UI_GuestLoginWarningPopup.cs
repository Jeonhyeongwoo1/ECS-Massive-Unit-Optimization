using System;
using MewVivor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_GuestLoginWarningPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _confirmButton;

        public void AddEvent(Action onCloseAction, Action onConfirmAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _confirmButton.SafeAddButtonListener(onConfirmAction.Invoke);
        }
    }
}