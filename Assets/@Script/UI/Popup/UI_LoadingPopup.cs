using System;
using UnityEngine;

namespace MewVivor.Popup
{
    public class UI_LoadingPopup : BasePopup
    {
        public static UI_LoadingPopup I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<UI_LoadingPopup>();
                }

                return _instance;
            }
        }
        
        private static UI_LoadingPopup _instance;
        
        [SerializeField] private Animator _animator;

        private static class AnimationName
        {
            public static int Show = Animator.StringToHash("Show");
            public static int Hide = Animator.StringToHash("Hide");
        }

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowAndHideLoadingPopup(bool isShow)
        {
            _animator.Play(isShow ? AnimationName.Show : AnimationName.Hide);
        }
    }
}