using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    [Serializable]
    public struct VipRewardData
    {
        public Sprite rewardSprite;
        public int amount;
    }

    public class UI_VipSubElement : UI_SubItemElement
    {
        [SerializeField] private TextMeshProUGUI _efficientText;
        [SerializeField] private List<UI_RewardItem> _rewardItemList;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TextMeshProUGUI _remainTimeText;
        [SerializeField] private OnClickScaler _onClickScaler;

        private string _itemId;

        public void AddEvent(Action<string> onPurchaseItemAction)
        {
            _purchaseButton.SafeAddButtonListener(() => onPurchaseItemAction.Invoke(_itemId));
        }

        public void UpdateUI(string itemId, string efficient, string price, List<VipRewardData> rewardDataList,
            bool isPossiblePurchase, string remainTime)
        {
            _itemId = itemId;
            _efficientText.text = $"x{efficient}";
            _priceText.text = price;

            for (var i = 0; i < rewardDataList.Count; i++)
            {
                VipRewardData rewardData = rewardDataList[i];
                UI_RewardItem item = _rewardItemList[i];

                item.UpdateUI(rewardData.rewardSprite, rewardData.amount);
            }

            _priceText.gameObject.SetActive(isPossiblePurchase);
            _remainTimeText.gameObject.SetActive(!isPossiblePurchase);
            _purchaseButton.interactable = isPossiblePurchase;
            _onClickScaler.IsOn = isPossiblePurchase;

            string desc = Manager.I.Data.LocalizationDataDict["Remaining_time"].GetValueByLanguage();
            _remainTimeText.text = $"{desc} : {remainTime}";
        }
    }
}