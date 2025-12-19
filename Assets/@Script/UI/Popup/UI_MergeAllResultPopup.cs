using System.Collections;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Popup
{
    public class UI_MergeAllResultPopup : BasePopup
    {
        public Transform MergeAlIScrollContentObject => _mergeAlIScrollContentObject;
        
        [SerializeField] private Transform _mergeAlIScrollContentObject;

        public void ReleaseEquipItem()
        {
            var childs = Utils.GetChildComponent<UI_EquipItem>(_mergeAlIScrollContentObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_EquipItem item in childs)
            {
                item.Release();
            }
        }
    }
}