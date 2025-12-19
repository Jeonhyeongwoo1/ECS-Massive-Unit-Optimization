using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_SkillDamageItem : MonoBehaviour
    {
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDamageText;
        [SerializeField] private Image _skillDamagePercentImage;
        [SerializeField] private TextMeshProUGUI _skillDamagePercentText;

        public void UpdateUI(float percent, string damage, string skillName, Sprite skillIcon)
        {
            _skillImage.sprite = skillIcon;
            _skillNameText.text = skillName;
            _skillDamageText.text = damage;
            _skillDamagePercentImage.fillAmount = percent;
            _skillDamagePercentText.text = $"{(percent * 100):0.00}%";
            gameObject.SetActive(true);
        }
    }
}