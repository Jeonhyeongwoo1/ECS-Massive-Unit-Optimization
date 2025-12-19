using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_RewardItem : UI_SubItemElement
    {
        [SerializeField] private Image _rewardImage;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;

        public void UpdateUI(Sprite rewardSprite, int rewardAmount)
        {
            if (rewardSprite != null)
            {
                _rewardImage.sprite = rewardSprite;
            }
            else
            {
                _rewardImage.sprite = null;
            }
            
            _rewardAmountText.text = $"x{rewardAmount}";
            gameObject.SetActive(true);
        }
    }
}