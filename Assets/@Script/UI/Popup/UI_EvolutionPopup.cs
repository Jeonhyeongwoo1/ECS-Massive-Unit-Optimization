using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Managers;
using MewVivor.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_EvolutionPopup : BasePopup
    {
        public int EvolutionItemCount => _evolutionItemList.Count;
        
        [SerializeField] private List<UI_EvolutionItem> _evolutionItemList;
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private TextMeshProUGUI _priceText;

        public void AddEvent(Action<UI_EvolutionItem> onShowToggleAction, Action onLevelUpAction)
        {
            foreach (UI_EvolutionItem item in _evolutionItemList)
            {
                item.AddEvent(onShowToggleAction);
            }
            
            _levelUpButton.SafeAddButtonListener(onLevelUpAction.Invoke);
        }

        public void UpdateUI(string priceValue, bool isPossibleUpdate, UserModel userModel)
        {
            _priceText.text = priceValue;
            _priceText.color = isPossibleUpdate ? Color.white : Color.red;

            DataManager dataManager = Manager.I.Data;
            List<EvolutionData> list = dataManager.EvolutionDataDict.Values.ToList();
            foreach (UI_EvolutionItem item in _evolutionItemList)
            {
                EvolutionData evolutionData = list.Find(v => v.EvolutionType == item.EvolutionType);
                int level = userModel.GetEvolutionLevel(evolutionData.EvolutionType);
                string description = evolutionData.GetDescription(level);
                string type = evolutionData.EvolutionType.ToString();
                bool isLock = level == 0;
                item.UpdateUI(level.ToString(), description, type, isLock);
            }
        }

        public int FindItemIndex(EvolutionOrderType evolutionType)
        {
            return _evolutionItemList.FindIndex(x => x.EvolutionOrderType == evolutionType);
        }

        public void ShowHighlight(int index, bool isActive)
        {
            _evolutionItemList[index].ShowHighlight(isActive);
        }

        public void ScaleUp(int index, bool isActive)
        {
            _evolutionItemList[index].ScaleUp(isActive);
        }
    }
}