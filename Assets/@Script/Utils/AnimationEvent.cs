using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MewVivor.Util
{
    public class AnimationEvent : MonoBehaviour
    {
        public UnityEvent _animationEvent;

        public void OnAnimationEvent()
        {
            _animationEvent.Invoke();
        }
    }
}