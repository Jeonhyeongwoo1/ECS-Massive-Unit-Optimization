using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Managers;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_MergeSuccessPopup : BasePopup
    {
        public enum MergeSuccessType
        {
            Merge2,
            Merge3,
            AllMerge
        }

        [Serializable]
        public struct MergeElement
        {
            public List<UI_MergeItem> equipItemList;
            public MergeSuccessType mergeSuccessType;
            public GameObject rootObject;
            public UI_MergeItem mergeSuccessEquipItem;
        }

        [SerializeField] private GameObject _mergeSuccessedObject;
        [SerializeField] private GameObject _mergeStartObject;
        [SerializeField] private GameObject _mergeEndObject;
        [SerializeField] private List<MergeElement> _mergeElementList;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TextMeshProUGUI _confirmText;
        [SerializeField] private Image _whiteflashImage;

        [SerializeField] private GameObject _selectMergeBGObject;
        [SerializeField] private GameObject _allMergeBGObject;
        [SerializeField] private GridLayoutGroup _userAddedEquipmentLayout;

        public void AddEvent(Action onCloseAction)
        {
            _closeButton.SafeAddButtonListener(onCloseAction.Invoke);
        }

        private MergeElement GetMergeElement(int count)
        {
            if (count == 2)
            {
                return _mergeElementList.Find(v => v.mergeSuccessType == MergeSuccessType.Merge2);
            }
            else if (count == 3)
            {
                return _mergeElementList.Find(v => v.mergeSuccessType == MergeSuccessType.Merge3);
            }
            else if (count >= 5)
            {
                return _mergeElementList.Find(v => v.mergeSuccessType == MergeSuccessType.AllMerge);
            }

            return _mergeElementList.Find(v => v.mergeSuccessType == MergeSuccessType.AllMerge);
        }

        public void UpdateUI(List<Equipment> mergedEquipmentList, List<UserEquipmentData> userAddedEquipmentDataList)
        {
            _whiteflashImage.DOFade(0, 0);
            _selectMergeBGObject.SetActive(true);
            _allMergeBGObject.SetActive(false);
            ResourcesManager resourcesManager = Manager.I.Resource;
            MergeElement mergeElement = GetMergeElement(mergedEquipmentList.Count);
            if (mergeElement.mergeSuccessType == MergeSuccessType.AllMerge)
            {
                _mergeStartObject.SetActive(true);
                foreach (MergeElement element in _mergeElementList)
                {
                    element.rootObject.SetActive(element.Equals(mergeElement));
                }

                
                for (var i = 0; i < mergeElement.equipItemList.Count; i++)
                {
                    if (mergedEquipmentList.Count <= i)
                    {
                        mergeElement.equipItemList[i].gameObject.SetActive(false);
                        continue;   
                    }
                    
                    Equipment equipment = mergedEquipmentList[i];
                    EquipmentType equipmentType = equipment.EquipmentType;
                    EquipmentData equipmentData = equipment.EquipmentData;
                    Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
                    Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                    Sprite gradeSprite =
                        Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.Grade);
                    mergeElement.equipItemList[i].UpdateUI(gradeSprite, equipSprite, equipTypeSprite, Vector3.one,
                        Utils.GetGradeCount(equipmentData.Grade));
                }
                
                StartCoroutine(AllMergeAnimationCor(mergeElement, userAddedEquipmentDataList));
            }
            else
            {
                _mergeStartObject.SetActive(true);
                foreach (MergeElement element in _mergeElementList)
                {
                    element.rootObject.SetActive(element.Equals(mergeElement));
                }

                {
                    EquipmentType equipmentType = mergedEquipmentList[0].EquipmentType;
                    EquipmentData equipmentData = mergedEquipmentList[0].EquipmentData;
                    Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
                    Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                    Sprite gradeSprite =
                        Const.EquipmentUIColors.GetEquipmentGradeSprite(mergedEquipmentList[0].Grade);
                    mergeElement.mergeSuccessEquipItem.gameObject.SetActive(false);
                    mergeElement.mergeSuccessEquipItem.UpdateUI(gradeSprite, equipSprite, equipTypeSprite,
                        Vector3.one * 1.2f, Utils.GetGradeCount(equipmentData.Grade));
                }

                for (var i = 0; i < mergeElement.equipItemList.Count; i++)
                {
                    Equipment equipment = mergedEquipmentList[i];
                    EquipmentType equipmentType = equipment.EquipmentType;
                    EquipmentData equipmentData = equipment.EquipmentData;
                    Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
                    Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                    Sprite gradeSprite =
                        Const.EquipmentUIColors.GetEquipmentGradeSprite(equipment.Grade);
                    mergeElement.equipItemList[i].UpdateUI(gradeSprite, equipSprite, equipTypeSprite, Vector3.one,
                        Utils.GetGradeCount(equipmentData.Grade));
                }

                StartCoroutine(SelectedMergeAnimationCor(mergeElement, userAddedEquipmentDataList));
            }
        }

        private IEnumerator AllMergeAnimationCor(MergeElement mergeElement,
            List<UserEquipmentData> userAddedEquipmentDataList)
        {
            _confirmText.gameObject.SetActive(false);
            _closeButton.interactable = false;
            _mergeSuccessedObject.SetActive(false);
            int index = 0;
            foreach (UI_MergeItem uiMergeItem in mergeElement.equipItemList)
            {
                StartCoroutine(uiMergeItem.MergeAnimationCor(mergeElement.mergeSuccessEquipItem.transform.position,
                    () => index++));
            }

            yield return new WaitUntil(() => mergeElement.equipItemList.Count == index);

            _whiteflashImage.DOFade(1, 0.3f);
            _selectMergeBGObject.SetActive(false);
            _allMergeBGObject.SetActive(true);
            
            ResourcesManager resourcesManager = Manager.I.Resource;
            var childs = _userAddedEquipmentLayout.GetComponentsInChildren<UI_MergeItem>();
            if (childs != null)
            {
                foreach (UI_MergeItem uiMergeItem in childs)
                {
                    uiMergeItem.Release();
                }
            }
            _mergeStartObject.SetActive(false);
            _mergeEndObject.SetActive(false);
            
            yield return new WaitForSeconds(1f);

            foreach (UserEquipmentData userEquipmentData in userAddedEquipmentDataList)
            {
                EquipmentType equipmentType = userEquipmentData.baseEquipmentType;
                EquipmentData equipmentData =
                    Manager.I.Data.EquipmentDataDict[userEquipmentData.baseItemId];
                Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
                Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
                Sprite gradeSprite =
                    Const.EquipmentUIColors.GetEquipmentGradeSprite(userEquipmentData.grade);
                var mergeItem = Manager.I.UI.AddSubElementItem<UI_MergeItem>(_userAddedEquipmentLayout.transform);
                mergeItem.UpdateUI(gradeSprite, equipSprite, equipTypeSprite, Vector3.one * 1.2f,
                    Utils.GetGradeCount(userEquipmentData.grade));
                mergeItem.gameObject.SetActive(true);
            }

            _whiteflashImage.DOFade(0, 0.3f);

            _mergeSuccessedObject.SetActive(true);
            _mergeSuccessedObject.transform.localScale = Vector3.zero;
            _mergeSuccessedObject.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            
            yield return new WaitForSeconds(0.3f);
            _closeButton.interactable = true;
            _confirmText.gameObject.SetActive(true);
            yield return null;
        }

        private IEnumerator SelectedMergeAnimationCor(MergeElement mergeElement, List<UserEquipmentData> userAddedEquipmentDataList)
        {
            _confirmText.gameObject.SetActive(false);
            _closeButton.interactable = false;
            _mergeSuccessedObject.SetActive(false);
            int index = 0;
            foreach (UI_MergeItem uiMergeItem in mergeElement.equipItemList)
            {
                StartCoroutine(uiMergeItem.MergeAnimationCor(mergeElement.mergeSuccessEquipItem.transform.position,
                    () => index++));
            }

            yield return new WaitUntil(() => mergeElement.equipItemList.Count == index);

            ResourcesManager resourcesManager = Manager.I.Resource;
            EquipmentType equipmentType = userAddedEquipmentDataList[0].baseEquipmentType;
            EquipmentData equipmentData = Manager.I.Data.EquipmentDataDict[userAddedEquipmentDataList[0].baseItemId];
            Sprite equipSprite = resourcesManager.Load<Sprite>(equipmentData.Sprite);
            Sprite equipTypeSprite = resourcesManager.Load<Sprite>($"{equipmentType}_Icon.sprite");
            Sprite gradeSprite =
                Const.EquipmentUIColors.GetEquipmentGradeSprite(userAddedEquipmentDataList[0].grade);
            mergeElement.mergeSuccessEquipItem.UpdateUI(gradeSprite, equipSprite, equipTypeSprite, Vector3.one * 1.2f,
                Utils.GetGradeCount(equipmentData.Grade));
            var item = mergeElement.mergeSuccessEquipItem;
            item.transform.localScale = Vector3.zero;
            item.gameObject.SetActive(true);
            item.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            _mergeSuccessedObject.SetActive(true);
            _mergeSuccessedObject.transform.localScale = Vector3.zero;
            _mergeSuccessedObject.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            _mergeStartObject.SetActive(false);
            _mergeEndObject.SetActive(true);

            _closeButton.interactable = true;
            _confirmText.gameObject.SetActive(true);
            yield return null;
        }
    }
}