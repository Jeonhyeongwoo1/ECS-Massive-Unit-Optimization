using MewVivor.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_MissionPopup : BasePopup
    {
        public Transform DailyMissionScrollObject => _dailyMissionScrollObject;

        [SerializeField] private Transform _dailyMissionScrollObject;
        [SerializeField] private ScrollRect _scrollRect;

        public void RefreshScrollView()
        {
            _scrollRect.verticalNormalizedPosition = 1;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        }
    }
}