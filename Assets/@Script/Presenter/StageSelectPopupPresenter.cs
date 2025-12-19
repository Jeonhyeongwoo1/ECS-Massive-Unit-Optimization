using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.UISubItemElement;
using UnityEngine;

namespace MewVivor.Presenter
{
    public class StageSelectPopupPresenter : BasePresenter
    {
        private UI_StageSelectPopup _popup;
        private DataManager _dataManager = Manager.I.Data;
        private UIManager _uiManager = Manager.I.UI;
        private int _currentStageIndex = 0;

        public void OpenStageSelectPopup()
        {
            _popup = _uiManager.OpenPopup<UI_StageSelectPopup>();
            _popup.onChangedSelectStageAction = OnChangedSelectStageAction;
            _popup.onSelectStageAction = OnSelectStage;
            _popup.onCloseStageSelectAction = OnCloseStageSelectPopup;
            _popup.AddEvents();

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            _currentStageIndex = Manager.I.SelectedStageIndex;
            StageData targetStageData = null;
            foreach (var (key, stageData) in _dataManager.StageDict)
            {
                if (_currentStageIndex == key)
                {
                    targetStageData = stageData;
                }
                
                UI_StageInfoItem stageInfoItem = _popup.StageInfoItemList[key - 1];
                int stageIndex = stageData.StageIndex;
                bool isOpenedStage = userModel.IsOpenedStage(stageIndex);
                stageInfoItem.UpdateUI(!isOpenedStage);
                _popup.AddChildObjectInHorizontalScrollSnap(stageInfoItem.gameObject);
            }

            var stageNameData = Manager.I.Data.LocalizationDataDict[targetStageData.StageNameKey];
            string titleValue = stageNameData.GetValueByLanguage();
            _popup.SetStartScreenPoint(_currentStageIndex - 1);
            _popup.UpdateUI(_currentStageIndex, titleValue, false, true);
        }
        
        private void OnChangedSelectStageAction(int index)
        {
            int stageIndex = index + 1;
            _currentStageIndex = stageIndex;
            bool isLastStage = _currentStageIndex == _popup.StageInfoItemList.Count;
            if (isLastStage)
            {
                _popup.UpdateUI(_currentStageIndex, null, true, false);
            }
            else
            {
                UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
                StageData targetStageData = Manager.I.Data.StageDict[_currentStageIndex];
                var stageNameData = Manager.I.Data.LocalizationDataDict[targetStageData.StageNameKey];
                string titleValue = stageNameData.GetValueByLanguage();
                bool isPossibleSelectStage = userModel.IsOpenedStage(_currentStageIndex);
                _popup.UpdateUI(_currentStageIndex, titleValue, false, isPossibleSelectStage);
            }
        }

        private void OnSelectStage()
        {
            _uiManager.ClosePopup();
            Manager.I.SelectedStageIndex = _currentStageIndex;

            var battlePopupPresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePopupPresenter.Refresh();
        }
        
        private void OnCloseStageSelectPopup()
        {
            _currentStageIndex = 0;
            _uiManager.ClosePopup();   
        }
    }
}