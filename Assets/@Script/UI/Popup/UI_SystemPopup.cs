using System;
using MewVivor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_SystemPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private TextMeshProUGUI _messageText;

        private Action _onPopupCloseAction;
        
        private void Start()
        {
            _closeButton.SafeAddButtonListener(() =>
            {
                _onPopupCloseAction?.Invoke();
                Manager.I.UI.ClosePopup();
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _onPopupCloseAction = null;
        }

        public void UpdateUI(string message)
        {
            _messageText.text = message;
        }

        public void AddEvent(Action onPopupCloseAction)
        {
            _onPopupCloseAction = onPopupCloseAction;
        }
    }
}