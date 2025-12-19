using System;
using MewVivor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_InfinityChallengePopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private TextMeshProUGUI _remainTicketCountText;
        [SerializeField] private TextMeshProUGUI _monsterKillCountText;
        [SerializeField] private TextMeshProUGUI _bestAliveTimeText;

        public void AddEvent(Action onGameStartAction, Action onCloseAction)
        {
            _gameStartButton.SafeAddButtonListener(onGameStartAction.Invoke);
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
        }

        public void UpdateUI(string remainTicketCount, string monsterKillCount, string bestAliveTime)
        {
            _remainTicketCountText.text = remainTicketCount;
            _monsterKillCountText.text = monsterKillCount;
            _bestAliveTimeText.text = bestAliveTime;
        }
    }
}