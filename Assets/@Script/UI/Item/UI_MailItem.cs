using System;
using System.Collections;
using MewVivor.Common;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.OutGame.UI
{
    public class UI_MailItem : UI_SubItemElement
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _remainTimeText;
        [SerializeField] private TextMeshProUGUI _itemAmountText;
        [SerializeField] private Button _claimButton;

        public void AddEvent(Action onClaimAction)
        {
            _claimButton.SafeAddButtonListener(onClaimAction.Invoke);
        }
        
        public void UpdateUI(Sprite itemSprite, string title, string description, int itemAmount, TimeSpan remainAt)
        {
            _itemImage.sprite = itemSprite;
            _titleText.text = title;
            _descriptionText.text = description;
            _itemAmountText.text = $"x{itemAmount}";
            string remainTime = "";
            if (remainAt.TotalDays >= 1)
            {
                int days = remainAt.Days;
                int hours = remainAt.Hours;
                
                remainTime = $"{days}d{hours}h";
            }
            else
            {
                int hours = remainAt.Hours;
                int minutes = remainAt.Minutes;
                
                remainTime = $"{hours}h{minutes}m";
            }
            
            _remainTimeText.text = remainTime;
            gameObject.SetActive(true);
            StartCoroutine(TimerCor(remainAt));
        }

        private IEnumerator TimerCor(TimeSpan remainAt)
        {
            float remainSeconds = (float)remainAt.TotalSeconds;

            while (remainSeconds > 0f)
            {
                remainSeconds -= Time.deltaTime;
                yield return null;
            }

            Manager.I.Pool.ReleaseObject(nameof(UI_MailItem), gameObject);
        }
    }
}