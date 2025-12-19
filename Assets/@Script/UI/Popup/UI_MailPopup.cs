using System;
using MewVivor.Common;
using MewVivor.OutGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_MailPopup : BasePopup
    {
        public Transform MailGroupTransform => _mailGroupTransform;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _allClaimButton;
        [SerializeField] private Transform _mailGroupTransform;
        [SerializeField] private GameObject _emptyMailObject;

        public void AddEvent(Action allClaimAction, Action onCloseAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _allClaimButton.SafeAddButtonListener(allClaimAction.Invoke);
            _bgCloseButton.SafeAddButtonListener(onCloseAction.Invoke);
        }

        public void ReleaseMailItem()
        {
            var childs = _mailGroupTransform.GetComponentsInChildren<UI_MailItem>();
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_MailItem uiMailItem in childs)
            {
                Manager.I.Pool.ReleaseObject(nameof(UI_MailItem), uiMailItem.gameObject);
            }
        }

        public void UpdateUI(bool isEmptyMail)
        {
            _emptyMailObject.SetActive(isEmptyMail);
            _allClaimButton.gameObject.SetActive(!isEmptyMail);
            _allClaimButton.interactable = !isEmptyMail;
        }
    }
}