using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace MewVivor.Popup
{
    public class UI_StageSelectPopup : BasePopup
    {
        public Transform StageScrollContentObject => _stageScrollContentObject;
        public List<UI_StageInfoItem> StageInfoItemList => _stageInfoItemList;
        
        [SerializeField] private HorizontalScrollSnap _stageSelectScrollView;
        [SerializeField] private Transform _stageScrollContentObject;
        [SerializeField] private Button _selectStageButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private List<UI_StageInfoItem> _stageInfoItemList;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private GameObject _titleObject;
        
        public Action onSelectStageAction;
        public Action onCloseStageSelectAction;
        public Action<int> onChangedSelectStageAction;

        public override void AddEvents()
        {
            base.AddEvents();
            
            _selectStageButton.SafeAddButtonListener(onSelectStageAction.Invoke);
            _backButton.SafeAddButtonListener(onCloseStageSelectAction.Invoke);
        }
        public void AddChildObjectInHorizontalScrollSnap(GameObject child)
        {
            _stageSelectScrollView.AddChild(child, new Vector2(507, 536));
        }

        public void UpdateUI(int stageIndex, string title, bool isLastStage, bool isPossibleSelectStage)
        {
            _titleText.text = $"{stageIndex}.{title}";
            // _stageSelectScrollView.StartingScreen = stageIndex;
            
            _titleObject.SetActive(!isLastStage);
            _selectStageButton.gameObject.SetActive(!isLastStage);
            _selectStageButton.interactable = isPossibleSelectStage;
        }

        public void SetStartScreenPoint(int startingScreen)
        {
            _stageSelectScrollView.StartingScreen = startingScreen;
        }

        public void OnChangedSelectStage(int index)
        {
            onChangedSelectStageAction.Invoke(_stageSelectScrollView.CurrentPage);
        }
    }
}