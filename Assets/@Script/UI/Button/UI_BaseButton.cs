using System;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UI
{
    public class UI_BaseButton : MonoBehaviour
    {
        [SerializeField] protected Button _button;
        [SerializeField] protected GameObject _redDotObject;

        protected virtual void Start()
        {
            SubscribeEvents();
        }

        protected virtual void SubscribeEvents()
        {
        }
    }
}