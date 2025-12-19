using System;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MewVivor.InGame.View
{
    public class UI_BossAndEliteMonsterStateInfo : MonoBehaviour
    {
        [FormerlySerializedAs("_nameText")] [SerializeField] private TextMeshProUGUI _remainText;
        [SerializeField] private Image _hpImage;

        public void AddEvents()
        {
            // var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            // stageModel.WaveTimer
            //     .Subscribe(OnChangedStageTimer)
            //     .AddTo(this);
        }
        
        private void OnEnable()
        {
            Manager.I.Event.AddEvent(GameEventType.TakeDamageEliteOrBossMonster, OnChangedRatio);
        }

        private void OnDisable()
        {
            Manager.I.Event.RemoveEvent(GameEventType.TakeDamageEliteOrBossMonster, OnChangedRatio);
        }
        
        private void OnChangedStageTimer(int time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            _remainText.text = timeSpan.ToString(@"mm\:ss");
        }
        
        private void OnChangedRatio(object value)
        {
            float ratio = (float)value;
            _hpImage.fillAmount = ratio;
        }
        
        public void UpdateMonsterInfo(string name, float ratio)
        {
            _remainText.text = name;
            _hpImage.fillAmount = ratio;
        }
    }
}