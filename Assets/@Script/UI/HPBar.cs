using System;
using System.Collections;
using System.Collections.Generic;
using MewVivor.InGame.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.InGame.View
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private PlayerController _player;
        
        private void OnEnable()
        {
            _player.onHitReceived += OnChangedValue;
            _player.onHealReceived += OnChangedValue;
        }

        private void OnChangedValue(int currentHp, int maxHp)
        {   
            float ratio = (float)currentHp / maxHp;
            _slider.value = ratio;
        }

        private void OnDisable()
        {
            
        }
    }
}