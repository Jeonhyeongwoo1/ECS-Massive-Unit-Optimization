using System;
using MewVivor.Data;
using MewVivor.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_EquipItem : UI_SubItemElement, IPointerClickHandler
    {
        [SerializeField] protected Image _equipmentGradeBackgroundImage;
        [SerializeField] protected Image _equipImage;
        [SerializeField] protected Image _equipTypeImage;
        [SerializeField] private TextMeshProUGUI _levelValueText;
        [SerializeField] private GameObject _upgradeableEquipArrowObject;
        [SerializeField] private GameObject _equipGradeNumberObject;
        [SerializeField] private TextMeshProUGUI _equipGradeNumberText;
        [SerializeField] private GameObject _emptyBlockObject;
        [SerializeField] private GameObject _selectedObject;
        [SerializeField] private GameObject _equippedObject;
        [SerializeField] private TextMeshProUGUI _equippedText;
        [SerializeField] private GameObject _redDotObject;
        [SerializeField] private OnClickScaler _onClickScaler;

        private Action _onClickEquipItemAction;

        public void AddEvent(Action onClickEquipItemAction)
        {
            _onClickEquipItemAction = onClickEquipItemAction;
        }

        public override void Release()
        {
            base.Release();
            _onClickEquipItemAction = null;
        }

        public void UpdateUI(Sprite equipSprite, Sprite equipTypeSprite, int level, Sprite gradeSprite,
            bool isDeactivate = false, int gradeCount = 0, bool isShowUpgradeableArrow = false)
        {
            _equipImage.sprite = equipSprite;
            _levelValueText.text = $"LV.{level}";
            _equipmentGradeBackgroundImage.sprite = gradeSprite;
            gameObject.SetActive(true);

            if (isDeactivate)
            {
                _levelValueText.gameObject.SetActive(false);
                _equipImage.gameObject.SetActive(false);
            }
            else
            {
                _levelValueText.gameObject.SetActive(true);
                _equipImage.gameObject.SetActive(true);
            }

            if (equipTypeSprite != null)
            {
                _equipTypeImage.enabled = true;
                _equipTypeImage.sprite = equipTypeSprite;
            }

            _equipGradeNumberObject.SetActive(gradeCount > 0);
            _equipGradeNumberText.text = gradeCount.ToString();
            _upgradeableEquipArrowObject.SetActive(isShowUpgradeableArrow);
        }

        public void ActivateScaler(bool isActive)
        {
            if (_onClickScaler == null)
            {
                return;
            }
            
            _onClickScaler.IsOn = isActive;
        }

        public void ActiveLevelText(bool isActive)
        {
            _levelValueText.gameObject.SetActive(isActive);
        }

        public void ActiveEquipment(bool isActive)
        {
            _equipImage.gameObject.SetActive(isActive);
            _levelValueText.gameObject.SetActive(isActive);
            _equipTypeImage.enabled= isActive;

            if (!isActive)
            {
                _equipmentGradeBackgroundImage.sprite =
                    Manager.I.Resource.Load<Sprite>(Const.EquipmentItem_Default_BG_Sprite);
                _equipGradeNumberObject.SetActive(false);
            }
        }

        public void UpdateMergeEquipmentUI(bool isEquipped, bool isSelected)
        {
            _equippedObject.SetActive(isEquipped);
            _selectedObject.SetActive(isSelected);
            _emptyBlockObject.SetActive(isEquipped);
            transform.localScale = Vector3.one;
        }

        public void ActiveEmptyBlockObject(bool isActive)
        {
            _emptyBlockObject.SetActive(isActive);
        }
        
        private void OnDisable()
        {
            if (_equippedObject != null)
            {
                _equippedObject.SetActive(false);
            }

            if (_selectedObject != null)
            {
                _selectedObject.SetActive(false);
            }

            if (_emptyBlockObject != null)
            {
                _emptyBlockObject.SetActive(false);
            }

            if (_redDotObject != null)
            {
                _redDotObject.SetActive(false);
            }
            
            _onClickEquipItemAction = null;
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClickEquipItemAction?.Invoke();
        }
    }
}