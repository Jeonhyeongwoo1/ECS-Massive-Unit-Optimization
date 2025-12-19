using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_HuntPassPopup : BasePopup
    {
        public Transform ScrollRectContentTransform => _scrollRect.content;
        public RectTransform LevelLineRectTransform => levelLineRectTransform;
        
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _premiumPassPurchaseButton;
        [SerializeField] private Image _fastBoostCurrentProgressbarImage;
        [SerializeField] private TextMeshProUGUI _fastBoostDestinationCountText;
        [FormerlySerializedAs("_levelLineRectTrasnform")] [SerializeField] private RectTransform levelLineRectTransform;
        [SerializeField] private GameObject _premiumPassPurchasedObject;

        public void AddEvent(Action onCloseHuntPassPopupAction, Action onPurchasePremiumPassPurchaseAction)
        {
            _closeButton.SafeAddButtonListener(onCloseHuntPassPopupAction.Invoke);
            _premiumPassPurchaseButton.SafeAddButtonListener(onPurchasePremiumPassPurchaseAction.Invoke);
        }

        public void Release()
        {
            var childs = ScrollRectContentTransform.GetComponentsInChildren<UI_HuntPassSubElement>();
            if (childs != null)
            {
                foreach (UI_HuntPassSubElement subElement in childs)
                {
                    Manager.I.Pool.ReleaseObject(nameof(UI_HuntPassSubElement), subElement.gameObject);
                }
            }
        }

        public void UpdateUI(string destinationLevel, float huntPassLevelPointRatio, bool isPremium)
        {
            _fastBoostDestinationCountText.text = destinationLevel;
            _fastBoostCurrentProgressbarImage.fillAmount = huntPassLevelPointRatio;

            _premiumPassPurchaseButton.interactable = !isPremium;
            _premiumPassPurchasedObject.SetActive(isPremium);
        }
    }
}