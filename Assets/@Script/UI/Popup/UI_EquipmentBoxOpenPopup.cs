using System;
using System.Collections;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_EquipmentBoxOpenPopup : BasePopup
    {
        [Serializable]
        public struct EquipmentElement
        {
            public EquipmentGrade equipmentGrade;
            public Color textColor;
        }
        
        public Transform PivotTransform => _pivotTransform;
        
        [SerializeField] private Button _confirmButton;
        [SerializeField] private GameObject _confirmTextObject;
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private GameObject _openBoxEffectObject;
        [SerializeField] private Animator _animator;
        [SerializeField] private List<EquipmentElement> _equipmentElementList;
        [SerializeField] private TextMeshProUGUI _equipmentGradeText;
        [SerializeField] private TextMeshProUGUI _equipmentNameText;
        [SerializeField] private GameObject _blinkObject;
        
         private List<UI_EquipItem> _equipItemList = new();
        private static int Box_OpenAnim = Animator.StringToHash("Box_Open");
        
        public void AddEvent(Action onConfirmAction)
        {
            _confirmButton.SafeAddButtonListener(onConfirmAction.Invoke);   
        }
        
        public void ReleaseEquipItem()
        {
            foreach (UI_EquipItem uiEquipItem in _equipItemList)
            {
                uiEquipItem.Release();
            }
            
            _equipItemList.Clear();
        }

        public void AddEquipItem(UI_EquipItem equipItem)
        {
            _equipItemList.Add(equipItem);
        }

        public void UpdateUI(EquipmentGrade equipmentGrade, string equipmentName)
        {
            _equipmentGradeText.text = Utils.GetEquipmentGrade(equipmentGrade);
            _equipmentNameText.text = equipmentName;
            _equipmentGradeText.color = GetEquipmentGradeColor(equipmentGrade);
            _equipmentNameText.color = GetEquipmentGradeColor(equipmentGrade);
            
            StartCoroutine(OpenBoxAnimationCor());
        }

        private Color GetEquipmentGradeColor(EquipmentGrade equipmentGrade)
        {
            var data = _equipmentElementList.Find(v => v.equipmentGrade == equipmentGrade);
            return data.textColor;
        }

        private IEnumerator OpenBoxAnimationCor()
        {
            _openBoxEffectObject.SetActive(false);
            _confirmButton.interactable = false;
            _confirmTextObject.gameObject.SetActive(false);
            _animator.Play(Box_OpenAnim);
            _equipmentGradeText.gameObject.SetActive(false);
            _equipmentNameText.gameObject.SetActive(false);
            _blinkObject.SetActive(false);
            yield return new WaitForSeconds(0.8f);
            _openBoxEffectObject.SetActive(true);

            foreach (UI_EquipItem uiEquipItem in _equipItemList)
            {
                uiEquipItem.gameObject.SetActive(true);
            }
            _equipmentGradeText.gameObject.SetActive(true);
            _equipmentNameText.gameObject.SetActive(true);
            _blinkObject.SetActive(true);

            yield return new WaitForSeconds(1f);
            _confirmButton.interactable = true;
            _confirmTextObject.gameObject.SetActive(true);
        }
    }
}