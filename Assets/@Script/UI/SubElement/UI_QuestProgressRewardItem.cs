using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    [Serializable]
    public struct TooltipRewardData
    {
        public Sprite sprite;
        public int rewardAmount;
    }
    
    public class UI_QuestProgressRewardItem : UI_SubItemElement
    {
        [Serializable]
        public struct TooltipData
        {
            public Image rewardImage;
            public TextMeshProUGUI rewardAmountText;
            public GameObject tooltipObject;
        }
        
        [SerializeField] private GameObject _tootipObject;
        [SerializeField] private TextMeshProUGUI _questDepthText;
        [SerializeField] private List<TooltipData> _tooltipDataList;
        [SerializeField] private Button _rewardButton;
        [SerializeField] private GameObject _openStateBoxObject;
        [SerializeField] private GameObject _closeStateBoxObject;
        [SerializeField] private GameObject _iconObject;
        [SerializeField] private GameObject _rewardIndicatorObject;

        private bool _isTooltipActive = false;
        private bool _isPossibleClaim;
        private bool _isFirstItem;
        private Action _onGetQuestStageRewardAction;
        
        public void AddEvent(Action onGetRewardAction)
        {
            _onGetQuestStageRewardAction = onGetRewardAction;
            _rewardButton.SafeAddButtonListener(OnGetReward);
        }

        private void OnGetReward()
        {
            if (_isPossibleClaim)
            {
                _onGetQuestStageRewardAction.Invoke();
            }
            else
            {
                if (_isFirstItem)
                {
                    return;
                }
                
                _isTooltipActive = !_isTooltipActive;
                _tootipObject.SetActive(_isTooltipActive);
            }
        }

        public void UpdateUI(bool isFirstItem, string questDepth, bool isClaim, bool isPossibleClaim, List<TooltipRewardData> tooltipRewardDataList)
        {
            _isFirstItem = isFirstItem;
            _isPossibleClaim = isPossibleClaim;
            _rewardButton.interactable = !isClaim && isPossibleClaim;
            _tootipObject.SetActive(false);
            _questDepthText.text = questDepth;
            if (tooltipRewardDataList != null)
            {
                for (var i = 0; i < _tooltipDataList.Count; i++)
                {
                    if (tooltipRewardDataList.Count <= i)
                    {
                        _tooltipDataList[i].tooltipObject.SetActive(false);
                        continue;
                    }
                    
                    TooltipRewardData rewardData = tooltipRewardDataList[i];
                    _tooltipDataList[i].rewardImage.sprite = rewardData.sprite;
                    _tooltipDataList[i].rewardAmountText.text = $"x{rewardData.rewardAmount}";
                    _tooltipDataList[i].tooltipObject.SetActive(true);
                }
            }

            _openStateBoxObject.SetActive(isClaim && !isFirstItem);
            _closeStateBoxObject.SetActive(!isClaim && !isFirstItem);
            _iconObject.SetActive(isFirstItem);
            _rewardIndicatorObject.SetActive(isPossibleClaim);
        }
    }
}