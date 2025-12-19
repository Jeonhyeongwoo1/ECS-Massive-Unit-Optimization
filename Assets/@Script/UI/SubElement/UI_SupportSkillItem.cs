using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_SupportSkillItem : MonoBehaviour
    {
        [SerializeField] private Button _supportSkillButton;
        [SerializeField] private Image _supportSkillIconImage;
        [SerializeField] private Image _bgSkillImage;

        private void Start()
        {
            _supportSkillButton.onClick.AddListener(OnShowSupportSkillToggle);
        }

        private void OnShowSupportSkillToggle()
        {
            
        }

        public void SetInfo(PassiveSkillData passiveSkillData, Transform parent)
        {
            _supportSkillIconImage.sprite = Manager.I.Resource.Load<Sprite>(passiveSkillData.IconLabel);
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        public void Release()
        {
            Manager.I.Pool.ReleaseObject(nameof(UI_SupportSkillItem), gameObject);
        }
    }
}