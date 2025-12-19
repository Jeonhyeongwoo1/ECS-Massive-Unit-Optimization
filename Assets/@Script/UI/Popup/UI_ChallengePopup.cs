using System;
using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.OutGame.UI;
using UnityEngine;

namespace MewVivor.Popup
{
    public class UI_ChallengePopup : BasePopup
    {
        [SerializeField] private List<UI_ChallengeItem> _challengeItemList;

        public void AddEvent(Action<ChallengeType> onShowChallengeAction)
        {
            _challengeItemList.ForEach(v => v.AddEvent(onShowChallengeAction));
        }

        public void UpdateUI(int infinityTicket)
        {
            int needCount = Const.InfinityTickCountForStartGame;
            UI_ChallengeItem infinityItem = _challengeItemList.Find(v => v.ChallengeType == ChallengeType.Infinity);
            infinityItem.UpdateUI($"{infinityTicket}/{needCount}", infinityTicket >= needCount);
        }
    }
}