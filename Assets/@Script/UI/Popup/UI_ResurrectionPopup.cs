using System;
using MewVivor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_ResurrectionPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _AdsButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _jewelCountText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameObject _continueLockObject;
        
        public void AddEvent(Action onShowAdsAction, Action onContinueGameAction, Action onGameDoneAction)
        {
            _closeButton.SafeAddButtonListener(()=> onGameDoneAction.Invoke());
            _AdsButton.SafeAddButtonListener(()=> onShowAdsAction.Invoke());
            _continueButton.SafeAddButtonListener(()=> onContinueGameAction.Invoke());
        }

        public void UpdateTimer(string timer)
        {
            _timerText.text = timer;
        }

        public void UpdateUI(string jewelValue, bool isPossibleContinue)
        {
            _jewelCountText.text = jewelValue;
            _continueLockObject.SetActive(!isPossibleContinue);
        }
    }
}