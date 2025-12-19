using System;
using MewVivor.Common;
using MewVivor.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_ExitGamePopup : BasePopup
    {
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] protected Button _closeButton;

        public void AddEvent(Action onQuickAction, Action onResumeAction, Action onCloseAction)
        {
            _quitButton.SafeAddButtonListener(()=>onQuickAction.Invoke());
            _resumeButton.SafeAddButtonListener(()=> onResumeAction.Invoke());
            _closeButton.SafeAddButtonListener(()=> onCloseAction.Invoke());
        }
    }
}