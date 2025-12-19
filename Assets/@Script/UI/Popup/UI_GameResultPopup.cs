using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Managers;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_GameResultPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _stageLevelText;
        [SerializeField] private TextMeshProUGUI _resultSurvivalTimeValueText;
        [SerializeField] private TextMeshProUGUI _killCountText;
        [SerializeField] private TextMeshProUGUI _bestSurvivalTimeValueText;
        [SerializeField] private GameObject _newRecordObject;
        [SerializeField] private Transform _resultRewardScrollContentObject;
        [SerializeField] private Button _statisticsButton;
        [SerializeField] private Button _confirmButton;

        public void AddEvent(Action onConfirmAction, Action onShowStatisticsAction)
        {
            _confirmButton.SafeAddButtonListener(()=> onConfirmAction.Invoke());
            _statisticsButton.SafeAddButtonListener(()=> onShowStatisticsAction.Invoke());
        }

        public void UpdateUI(string stageResult, 
                            string gameplayTime, 
                            int totalMonsterKillCount, 
                            string bestSurvivalTime,
                            bool isNewRecord,
                            GameType gameType,
                            Dictionary<int, int> rewardItemDict)
        {
            _stageLevelText.text = gameType == GameType.MAIN ? $"Stage {stageResult}" : stageResult;
            _resultSurvivalTimeValueText.text = gameplayTime;
            _killCountText.text = totalMonsterKillCount.ToString();
            _bestSurvivalTimeValueText.text = bestSurvivalTime;
            _newRecordObject.SetActive(isNewRecord);
            
            DataManager dataManager = Manager.I.Data;
            UIManager uiManager = Manager.I.UI;
            ResourcesManager resourcesManager = Manager.I.Resource;
            foreach (var (id, amount) in rewardItemDict)
            {
                ItemData data = dataManager.ItemDataDict[id];
                var sprite = resourcesManager.Load<Sprite>(data.SpriteName);
                int count = amount;

                var materialItem =
                    uiManager.AddSubElementItem<UI_MaterialItem>(_resultRewardScrollContentObject);
                materialItem.UpdateUI(sprite, Color.blue, count.ToString(), true);
                materialItem.transform.localScale = Vector3.one;
            }

            // Manager.I.Resource.Instantiate();
        }
    }
}