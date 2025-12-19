using System;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.Key;
using MewVivor.Managers;
using MewVivor.UISubItemElement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] protected Button _bgCloseButton;
        
        public bool IsInitialize { get; protected set; }

        public virtual void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }
         
            if (_bgCloseButton != null)
            {
                _bgCloseButton.SafeAddButtonListener(() => Manager.I.UI.ClosePopup());
            }
            
            IsInitialize = true;
        }

        protected virtual void OnDisable()
        {
            
        }

        public virtual void AddEvents()
        {
            
        }

        protected void SafeButtonAddListener(ref Button button, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        public void ReleaseSubItem<T>(Transform parent) where T : UI_SubItemElement
        {
            var childs = Utils.GetChildComponent<T>(parent);
            if (childs == null)
            {
                return;
            }
            
            foreach (T subItem in childs)
            {
                subItem.Release();
            }
        }

        public virtual void OpenPopup()
        {
            Manager.I.Audio.Play(Sound.SFX, SoundKey.Popup_Open);
            gameObject.SetActive(true);
        }

        public virtual void ClosePopup()
        {
            PlayPopupCommonCloseSound();
            ReleaseObject();
        }

        private void ReleaseObject()
        {
            Manager.I.Pool.ReleaseObject(gameObject.name, gameObject);
        }

        private void PlayPopupCommonCloseSound()
        {
            // Manager.I.Audio.Play(Sound.SFX, "PopupClose_Common");
        }
    }
}