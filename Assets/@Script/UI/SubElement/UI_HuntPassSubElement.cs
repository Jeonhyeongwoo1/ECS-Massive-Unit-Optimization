using System;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_HuntPassSubElement : UI_SubItemElement
    {
        [Serializable]
        public struct PassSubElementData
        {
            public TextMeshProUGUI rewardAmountText;
            public Image rewardImage;
            public GameObject lockObject;
            public GameObject highlightObject;
            public GameObject redDotObject;
            public Button claimButton;
            // public OnClickScaler onClickScaler;
        }

        [SerializeField] private PassSubElementData _normalPassSubElementData;
        [SerializeField] private PassSubElementData _premiumPassSubElementData;
        [SerializeField] private TextMeshProUGUI _levelText;

        public void AddEvent(Action<bool, int> onGetRewardAction, int level)
        {
            _normalPassSubElementData.claimButton.SafeAddButtonListener(()=> onGetRewardAction.Invoke(false, level));
            _premiumPassSubElementData.claimButton.SafeAddButtonListener(()=> onGetRewardAction.Invoke(true , level));
        }
        
        public void UpdateUI(PassRewardData passRewardData, HuntPassData huntPassData)
        {
            UpdateNormalPassSubElement(passRewardData, huntPassData);
            UpdatePremiumSubElement(passRewardData, huntPassData);
            
            _levelText.text = passRewardData.Level.ToString();
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        private void UpdatePremiumSubElement(PassRewardData passRewardData, HuntPassData huntPassData)
        {
            int? claimableLevel = huntPassData.premiumClaimableLevel;
            int lastReceivedLevel = huntPassData.lastReceivedPremiumLevel;
            bool isLock = false;
            bool isClaimable = false;
            bool isGetReward = false;
            if (claimableLevel.HasValue)
            {
                if (claimableLevel.Value >= passRewardData.Level)
                {
                    if (lastReceivedLevel < passRewardData.Level && huntPassData.isPremium)
                    {
                        isClaimable = true;
                    }
                    else if(huntPassData.isPremium)
                    {
                        isClaimable = false;
                        isGetReward = true;
                    }
                }
                else
                {
                    isLock = true;
                    isClaimable = false;
                }
            }
            else
            {
                isLock = true;
                isClaimable = false;
            }

            if (!huntPassData.isPremium)
            {
                isLock = true;
            }
            
            // Debug.Log(
            //     $"index {passRewardData.Level} / isLock {isLock} / isClaimable {isClaimable} / isGetReward {isGetReward}");

            int rewardAmount = passRewardData.PaidReward_Amount;
            ItemData rewardItemData = Manager.I.Data.ItemDataDict[passRewardData.PaidReward_Id];
            Sprite rewardSprite = Manager.I.Resource.Load<Sprite>(rewardItemData.SpriteName);
            _premiumPassSubElementData.rewardImage.rectTransform.sizeDelta =
                Const.ID_GOLD == passRewardData.PaidReward_Id ? new Vector2(128, 128) : new Vector2(164, 164);
            
            // _premiumPassSubElementData.onClickScaler.IsOn = isClaimable;
            _premiumPassSubElementData.claimButton.interactable = isClaimable;
            _premiumPassSubElementData.lockObject.SetActive(isLock);
            _premiumPassSubElementData.highlightObject.SetActive(isGetReward);
            _premiumPassSubElementData.redDotObject.SetActive(isClaimable);
            _premiumPassSubElementData.rewardImage.sprite = rewardSprite;
            _premiumPassSubElementData.rewardAmountText.text = $"x{rewardAmount}";
        }

        private void UpdateNormalPassSubElement(PassRewardData passRewardData, HuntPassData huntPassData)
        {
            int? claimableLevel = huntPassData.normalClaimableLevel;
            int lastReceivedLevel = huntPassData.lastReceivedLevel;
            bool isLock = false;
            bool isClaimable = false;
            bool isGetReward = false;
            if (claimableLevel.HasValue)
            {
                if (claimableLevel.Value >= passRewardData.Level)
                {
                    if (lastReceivedLevel < passRewardData.Level)
                    {
                        isClaimable = true;
                    }
                    else
                    {
                        isClaimable = false;
                        isGetReward = true;
                    }
                }
                else
                {
                    isLock = true;
                    isClaimable = false;
                }
            }
            else
            {
                isLock = true;
                isClaimable = false;
            }

            int normalRewardAmount = passRewardData.FreeReward_Amount;
            ItemData rewardItemData = Manager.I.Data.ItemDataDict[passRewardData.FreeReward_Id];
            Sprite rewardSprite = Manager.I.Resource.Load<Sprite>(rewardItemData.SpriteName);

            _normalPassSubElementData.rewardImage.rectTransform.sizeDelta =
                Const.ID_GOLD == passRewardData.FreeReward_Id ? new Vector2(128, 128) : new Vector2(164, 164);
            //일단 노멀 기준으로만
            // _normalPassSubElementData.onClickScaler.IsOn = isClaimable;
            _normalPassSubElementData.claimButton.interactable = isClaimable;
            _normalPassSubElementData.lockObject.SetActive(isLock);
            _normalPassSubElementData.highlightObject.SetActive(isGetReward);
            _normalPassSubElementData.redDotObject.SetActive(isClaimable);
            _normalPassSubElementData.rewardImage.sprite = rewardSprite;
            _normalPassSubElementData.rewardAmountText.text = $"x{normalRewardAmount}";
            transform.SetAsLastSibling();
        }
    }
}