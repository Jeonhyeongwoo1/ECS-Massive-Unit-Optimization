using MewVivor.Data;
using MewVivor.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_SupportCardItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;

        public void SetInfo(PassiveSkillData passiveSkillData)
        {
            // _skillImage.sprite = Manager.I.Resource.Load<Sprite>(passiveSkillData.IconLabel);
            // _skillNameText.text = passiveSkillData.Name;
            // _skillDescriptionText.text = passiveSkillData.Description;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}