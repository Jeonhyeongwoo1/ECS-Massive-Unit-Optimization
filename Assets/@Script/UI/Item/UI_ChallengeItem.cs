using System;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.OutGame.UI
{
    public class UI_ChallengeItem : UI_SubItemElement
    {
        public ChallengeType ChallengeType => _challengeType;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _ticketCountText;
        [SerializeField] private ChallengeType _challengeType;
        
        public void AddEvent(Action<ChallengeType> onShowChallengeAction)
        {
            _button.SafeAddButtonListener(() => onShowChallengeAction.Invoke(_challengeType));
        }

        public void UpdateUI(string ticketAmount, bool isPossibleStartGame)
        {
            _ticketCountText.text = ticketAmount;
            _ticketCountText.color = isPossibleStartGame ? Color.white : Color.red;
            _button.interactable = isPossibleStartGame;
        }
    }
}