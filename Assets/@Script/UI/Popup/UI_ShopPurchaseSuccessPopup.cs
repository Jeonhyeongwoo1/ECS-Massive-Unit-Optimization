using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.UISubItemElement;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_ShopPurchaseSuccessPopup : BasePopup
    {
        public Transform ContentTransform => _contentTrnasform;
        
        [SerializeField] private Transform _contentTrnasform;
        [SerializeField] private Button _confirmButton;

        public void AddEvent(Action onConfirmAction)
        {
            _confirmButton.SafeAddButtonListener(onConfirmAction.Invoke);
        }

        public void ReleaseItem()
        {
            var shopPurchaseChilds = _contentTrnasform.GetComponentsInChildren<UI_ShopPurchaseSuccessItem>();
            if (shopPurchaseChilds !=  null)
            {
                foreach (var item in shopPurchaseChilds)
                {
                    Manager.I.Pool.ReleaseObject(nameof(UI_ShopPurchaseSuccessItem),
                        item.gameObject);
                }
            }
            
            var childs = _contentTrnasform.GetComponentsInChildren<UI_EquipItem>();
            if (childs !=  null)
            {
                foreach (var item in childs)
                {
                    Manager.I.Pool.ReleaseObject(nameof(UI_EquipItem),
                        item.gameObject);
                }
            }
        }
    }
}