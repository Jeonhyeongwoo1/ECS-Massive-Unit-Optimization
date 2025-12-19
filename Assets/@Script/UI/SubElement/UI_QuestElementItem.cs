using System;
using MewVivor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_QuestElementItem : UI_SubItemElement
    {
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _progressDescriptionText;
        [SerializeField] private Image _progressbarImage;
        [SerializeField] private UI_RewardItem _rewardItem;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Image _questBoxBGImage;
        [SerializeField] private Image _rewardBoxImage;

        [SerializeField] private Sprite _normalStateQuestBoxSprite;
        [SerializeField] private Sprite _clearStateQuestBoxSprite;
        [SerializeField] private Sprite _normalStateRewardBoxSprite;
        [SerializeField] private Sprite _clearStateRewardBoxSprite;

        public void AddEvent(Action onGetRewardAction)
        {
            _claimButton.SafeAddButtonListener(onGetRewardAction.Invoke);
        }

        public void UpdateUI(string description, float ratio, Sprite rewardSprite, int rewardAmount, bool isPossibleGetReward, string progress)
        {
            _descriptionText.text = description;
            _progressDescriptionText.text = progress;
            _progressbarImage.fillAmount = ratio;
            _rewardItem.UpdateUI(rewardSprite, rewardAmount);
            _claimButton.interactable = isPossibleGetReward;

            _questBoxBGImage.sprite = isPossibleGetReward ? _clearStateQuestBoxSprite : _normalStateQuestBoxSprite;
            _rewardBoxImage.sprite = isPossibleGetReward ? _clearStateRewardBoxSprite : _normalStateRewardBoxSprite;
            gameObject.SetActive(true);
        }
    }
}