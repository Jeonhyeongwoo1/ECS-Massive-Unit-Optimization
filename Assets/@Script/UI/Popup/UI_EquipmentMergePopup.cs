using System;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.UISubItemElement;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_EquipmentMergePopup : BasePopup
    {
        [Serializable]
        public struct OneMergeCountSlot
        {
            public UI_EquipItem SelectedEquipmentItem;
            public UI_EquipItem MergeEquipmentItem1;
            public UI_EquipItem ResultEquipmentItem;
            public Image blinkImage;
            public GameObject slotObject;
        }
        
        [Serializable]
        public struct TwoMergeCountSlot
        {
            public UI_EquipItem SelectedEquipmentItem;
            public UI_EquipItem MergeEquipmentItem1;
            public UI_EquipItem MergeEquipmentItem2;
            public UI_EquipItem ResultEquipmentItem;
            public Image blinkImage;
            public GameObject slotObject;
        }

        public Transform EquipmentGroupTransform => _equipmentGroupTransform;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private OneMergeCountSlot _oneMergeCountSlot;
        [SerializeField] private TwoMergeCountSlot _twoMergeCountSlot;
        [SerializeField] private TextMeshProUGUI _equipmentName;
        [SerializeField] private TextMeshProUGUI _beforeMaxLevelText;
        [SerializeField] private TextMeshProUGUI _afterMaxLevelText;
        [SerializeField] private TextMeshProUGUI _beforeStatLevelText;
        [SerializeField] private TextMeshProUGUI _afterStatLevelText;
        [SerializeField] private TextMeshProUGUI _statText;
        [SerializeField] private Button _mergeButton;
        [SerializeField] private GameObject _statInfoFrameObject;
        [SerializeField] private Button _allMergeButton;
        [SerializeField] private Transform _equipmentGroupTransform;
        
        [SerializeField] private Button _sortButton;
        [SerializeField] private TextMeshProUGUI _sortByLevelText;
        [SerializeField] private TextMeshProUGUI _sortQualityText;
        [SerializeField] private GameObject _sortByLevelActiveObject;
        [SerializeField] private GameObject _sortQualityActiveObject;
        [SerializeField] private GameObject _mergeAllRedDotObject;
        
        private Tween blinkTween;
        private Action<Equipment> _onClickMergeEquipmentItemAction;
        private CompositeDisposable _subscriptionDisposables;

        protected override void OnDisable()
        {
            base.OnDisable();
            
            blinkTween?.Kill();
            blinkTween = null;
        }

        private void OnDestroy()
        {
            _subscriptionDisposables?.Dispose();
        }

        public void AddEvent(Action onCloseAction, Action onMergeAction, Action onAllMergeAction,
            Action onSortEquipItemAction, Action<Equipment> onClickMergeEquipmentItemAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
            _mergeButton.SafeAddButtonListener(onMergeAction.Invoke);
            _allMergeButton.SafeAddButtonListener(onAllMergeAction.Invoke);
            _sortButton.SafeAddButtonListener(onSortEquipItemAction.Invoke);
            _onClickMergeEquipmentItemAction = onClickMergeEquipmentItemAction;
            
            if (_subscriptionDisposables == null)
            {
                _subscriptionDisposables = new CompositeDisposable();
                var userModel = ModelFactory.CreateOrGetModel<UserModel>();
                userModel.IsPossibleMerge.Subscribe(x =>
                    {
                        _mergeButton.gameObject.SetActive(x);
                    })
                    .AddTo(_subscriptionDisposables);
                userModel.IsPossibleMergeAll.Subscribe(x =>
                {
                    _allMergeButton.gameObject.SetActive(x);
                    _mergeAllRedDotObject.SetActive(x);
                }).AddTo(_subscriptionDisposables);
            }
        }

        public void UpdateUI(int mergeCount, Equipment selectedEquipment, Equipment mergeEquipment1,
            Equipment mergeEquipment2, EquipmentData resultEquipmentData, EquipmentSortType equipmentSortType)
        {
            _oneMergeCountSlot.slotObject.gameObject.SetActive(mergeCount == 1);
            _twoMergeCountSlot.slotObject.gameObject.SetActive(mergeCount == 2);
            bool isOneMergeCount = mergeCount == 1;
            ResourcesManager resourcesManager = Manager.I.Resource;
            if (selectedEquipment != null)
            {
                EquipmentType equipmentType = selectedEquipment.EquipmentData.EquipmentType;
                Sprite equipSprite = resourcesManager.Load<Sprite>(selectedEquipment.EquipmentData.Sprite);
                int level = selectedEquipment.Level;
                Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                Sprite gradeSprite =
                    Const.EquipmentUIColors.GetEquipmentGradeSprite(selectedEquipment.EquipmentData.Grade);

                UI_EquipItem targetEquipItem = isOneMergeCount
                    ? _oneMergeCountSlot.SelectedEquipmentItem
                    : _twoMergeCountSlot.SelectedEquipmentItem;
                targetEquipItem.AddEvent(() => _onClickMergeEquipmentItemAction.Invoke(selectedEquipment));
                targetEquipItem.UpdateUI(equipSprite, equipTypeSprite, level, gradeSprite,
                    false, selectedEquipment.GetGradeCount());
                targetEquipItem.transform.localScale = Vector3.one;

                LocalizationData data = Manager.I.Data.LocalizationDataDict[selectedEquipment.EquipmentData.NameTextID];
                _equipmentName.text = data.GetValueByLanguage();
                _allMergeButton.gameObject.SetActive(false);
            }
            else
            {
                var userModel = ModelFactory.CreateOrGetModel<UserModel>();
                _allMergeButton.gameObject.SetActive(userModel.IsPossibleMergeAll.Value);
                _equipmentName.text = "";
            }

            if (mergeEquipment1 != null)
            {
                EquipmentType equipmentType = mergeEquipment1.EquipmentData.EquipmentType;
                Sprite equipSprite = resourcesManager.Load<Sprite>(mergeEquipment1.EquipmentData.Sprite);
                int level = mergeEquipment1.Level;
                Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                Sprite gradeSprite =
                    Const.EquipmentUIColors.GetEquipmentGradeSprite(mergeEquipment1.EquipmentData.Grade);
                
                UI_EquipItem targetEquipItem = isOneMergeCount
                    ? _oneMergeCountSlot.MergeEquipmentItem1
                    : _twoMergeCountSlot.MergeEquipmentItem1;
                targetEquipItem.AddEvent(() => _onClickMergeEquipmentItemAction.Invoke(mergeEquipment1));
                targetEquipItem.UpdateUI(equipSprite, equipTypeSprite, level, gradeSprite,
                    false, mergeEquipment1.GetGradeCount());
                targetEquipItem.transform.localScale = Vector3.one;
            }
            else
            {
                _oneMergeCountSlot.MergeEquipmentItem1.ActiveEquipment(false);
                _twoMergeCountSlot.MergeEquipmentItem1.ActiveEquipment(false);
            }
            
            if (!isOneMergeCount)
            {
                if (mergeEquipment2 != null)
                {
                    EquipmentType equipmentType = mergeEquipment2.EquipmentData.EquipmentType;
                    Sprite equipSprite = resourcesManager.Load<Sprite>(mergeEquipment2.EquipmentData.Sprite);
                    int level = mergeEquipment2.Level;
                    Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                    Sprite gradeSprite =
                        Const.EquipmentUIColors.GetEquipmentGradeSprite(mergeEquipment2.EquipmentData.Grade);
                    _twoMergeCountSlot.MergeEquipmentItem2.AddEvent(() => _onClickMergeEquipmentItemAction.Invoke(mergeEquipment2));
                    _twoMergeCountSlot.MergeEquipmentItem2.UpdateUI(equipSprite, equipTypeSprite, level, gradeSprite,
                        false, mergeEquipment2.GetGradeCount());
                }
                else
                {
                    _twoMergeCountSlot.MergeEquipmentItem2.ActiveEquipment(false);
                }
            }

            if (resultEquipmentData != null)
            {
                EquipmentType equipmentType = resultEquipmentData.EquipmentType;
                Sprite equipSprite = resourcesManager.Load<Sprite>(resultEquipmentData.Sprite);
                int level = selectedEquipment == null ? 0 : selectedEquipment.Level;
                Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                Sprite gradeSprite =
                    Const.EquipmentUIColors.GetEquipmentGradeSprite(resultEquipmentData.Grade);
                
                UI_EquipItem targetEquipItem = isOneMergeCount
                    ? _oneMergeCountSlot.ResultEquipmentItem
                    : _twoMergeCountSlot.ResultEquipmentItem;
                targetEquipItem.UpdateUI(equipSprite, equipTypeSprite, level, gradeSprite,
                    false, Utils.GetGradeCount(resultEquipmentData.Grade));
               
                Image blinkImage = isOneMergeCount
                    ? _oneMergeCountSlot.blinkImage
                    : _twoMergeCountSlot.blinkImage;
                blinkImage.gameObject.SetActive(true);
                if (blinkTween == null)
                {
                    // 알파를 0으로 갔다가 다시 1로 반복 (무한 루프)
                    blinkTween = blinkImage.DOFade(0f, 0.5f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }
            }
            else
            {
                Image blinkImage = isOneMergeCount
                    ? _oneMergeCountSlot.blinkImage
                    : _twoMergeCountSlot.blinkImage;
                blinkImage.gameObject.SetActive(false);
                blinkTween?.Kill();
                _oneMergeCountSlot.ResultEquipmentItem.ActiveEquipment(false);
                _twoMergeCountSlot.ResultEquipmentItem.ActiveEquipment(false);
            }

            if (selectedEquipment != null && resultEquipmentData != null)
            {
                _beforeMaxLevelText.text = selectedEquipment.EquipmentData.Grade_MaxLevel.ToString();
                _afterMaxLevelText.text = resultEquipmentData.Grade_MaxLevel.ToString();

                int beforeStat = selectedEquipment.EquipmentData.Grade_Atk == 0
                    ? selectedEquipment.EquipmentData.Grade_Hp
                    : selectedEquipment.EquipmentData.Grade_Atk;
                
                int afterStat = resultEquipmentData.Grade_Atk == 0
                    ? resultEquipmentData.Grade_Hp
                    : resultEquipmentData.Grade_Atk;
                _beforeStatLevelText.text = beforeStat.ToString();
                _afterStatLevelText.text = afterStat.ToString();
                _statText.text = resultEquipmentData.Grade_Atk == 0
                    ? "HP"
                    : "ATK";   
                _statInfoFrameObject.SetActive(true);
            }
            else
            {
                _statInfoFrameObject.SetActive(false);
            }
            
            _sortByLevelText.color = equipmentSortType == EquipmentSortType.Level
                ? Const.MergeSortTypeActiveColor
                : Const.MergeSortTypeDeactiveColor;
            _sortQualityText.color = equipmentSortType == EquipmentSortType.Grade
                ? Const.MergeSortTypeActiveColor
                : Const.MergeSortTypeDeactiveColor;
            _sortByLevelActiveObject.SetActive(equipmentSortType == EquipmentSortType.Level);
            _sortQualityActiveObject.SetActive(equipmentSortType == EquipmentSortType.Grade);
        }

        public void ReleaseEquipItems()
        {
            var childs = Utils.GetChildComponent<UI_EquipItem>(_equipmentGroupTransform);
            if (childs == null)
            {
                return;
            }

            foreach (UI_EquipItem uiEquipItem in childs)
            {
                uiEquipItem.Release();
            }
        }
    }
}