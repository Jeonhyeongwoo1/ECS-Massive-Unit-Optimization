using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor.Presenter
{
    public class EvolutionPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private UI_EvolutionPopup _popup;
        private UI_EvolutionItem _toggleActivatedEvolutionItem;

        public void OpenPopup()
        {
            _userModel = ModelFactory.CreateOrGetModel<UserModel>();
            _popup = Manager.I.UI.OpenPopup<UI_EvolutionPopup>();
            _popup.AddEvent(OnShowToggle, OnLevelUp);
            Refresh();
        }

        private async void OnLevelUp()
        {
            if (TaskHelper.IsTcsTasking())
            {
                return;
            }
            
            int level = _userModel.UserData.evolutionCount;
            int price = CalculatePrice(level);
            if (_userModel.Gold.Value < price)
            {
                string message = Manager.I.Data.LocalizationDataDict["NOT_Enought_Gold"].GetValueByLanguage();
                Manager.I.UI.OpenSystemPopup(message);
                return;
            }

            TaskHelper.InitTcs();
            var response =
                await Manager.I.Web.SendRequest<EvolutionLevelUpResponseData>($"/user/evolution", null,
                    MethodType.PATCH.ToString());
            TaskHelper.CompleteTcs();
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                return;
            }

            EvolutionOrderType evolutionOrderType = response.data.updatedEvolutionStat;
            _userModel.SetUserData(response.data.userData);
            _userModel.SetInventory(response.data.inventory);
            ShowLevelUpAnimation(evolutionOrderType).Forget();
        }

        private async UniTaskVoid ShowLevelUpAnimation(EvolutionOrderType evolutionOrderType)
        {
            UI_BlockCanvas.I.ShowAndHideBlockCanvas(true);

            int count = 30;
            List<int> indexList = new List<int>(count);
            int elementLength = _popup.EvolutionItemCount;
            for (int i = 0; i < count; i++)
            {
                while (true)
                {
                    int random = Random.Range(0, elementLength);
                    if (indexList.Count == 0)
                    {
                        indexList.Add(random);
                        break;
                    }

                    int prevIndex = indexList[^1];
                    if (prevIndex == random)
                    {
                        continue;
                    }
                    
                    indexList.Add(random);
                    break;
                }
            }

            int lastIndex = _popup.FindItemIndex(evolutionOrderType);
            indexList.Add(lastIndex);
            await ShowHighlightAsync(indexList);
            UI_BlockCanvas.I.ShowAndHideBlockCanvas(false);
        }

        private async UniTask ShowHighlightAsync(List<int> indexList)
        {
            int prevIndex = 0;
            for (int i = 0; i < indexList.Count; i++)
            {
                int index = indexList[i];
                _popup.ShowHighlight(index, true);
                _popup.ShowHighlight(prevIndex, false);
                prevIndex = index;

                float lerp = Mathf.Lerp(0, 1, (float)i / indexList.Count);
                float duration = Mathf.Lerp(0.2f, 0.03f, lerp);
                await UniTask.WaitForSeconds(duration);
            }
            
            _popup.ScaleUp(indexList[^1], true);
            await UniTask.WaitForSeconds(0.6f);
            _popup.ShowHighlight(indexList[^1], false);
            _popup.ScaleUp(indexList[^1], false);
            Refresh();
        }

        private void OnShowToggle(UI_EvolutionItem item)
        {
            if (item == _toggleActivatedEvolutionItem)
            {
                _toggleActivatedEvolutionItem.ActiveTooltip(false);
                _toggleActivatedEvolutionItem = null;
                return;
            }
            
            if (_toggleActivatedEvolutionItem != null)
            {
                _toggleActivatedEvolutionItem.ActiveTooltip(false);
            }

            _toggleActivatedEvolutionItem = item;
            _toggleActivatedEvolutionItem.ActiveTooltip(true);
        }

        public void ClosePopup()
        {
            if (_popup == null)
            {
                return;
            }
            
            Manager.I.UI.ClosePopup();
        }

        private void Refresh()
        {
            int level = _userModel.UserData.evolutionCount;
            EvolutionOrderTypeByServer type = _userModel.UserData.evolutionOrderType;
            int price = CalculatePrice(level);
            bool isPossibleUpdate = _userModel.Gold.Value >= price;
            _popup.UpdateUI(price.ToString(), isPossibleUpdate, _userModel);
        }

        public int CalculatePrice(int level)
        {   
            DataManager dataManager = Manager.I.Data;
            GlobalConfigData evolutionDefaultCostData =
                dataManager.GlobalConfigDataDict[GlobalConfigName.EvolutionDefaultCost];
            GlobalConfigData evolutionCostIncreaseData =
                dataManager.GlobalConfigDataDict[GlobalConfigName.EvolutionCostIncrease];
            GlobalConfigData evolutionMaxCostData =
                dataManager.GlobalConfigDataDict[GlobalConfigName.EvolutionMaxCost];

            float evolutionDefaultCost = evolutionDefaultCostData.Value;
            float evolutionCostIncrease = evolutionCostIncreaseData.Value;
            float evolutionMaxCost = evolutionMaxCostData.Value;

            int price = (int)Mathf.Min(evolutionDefaultCost + evolutionCostIncrease * level, evolutionMaxCost);
            return price;
        }
    }   
}