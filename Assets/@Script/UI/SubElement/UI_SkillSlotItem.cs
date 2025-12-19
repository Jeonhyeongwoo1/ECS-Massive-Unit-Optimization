using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.UISubItemElement
{
    public class UI_SkillSlotItem : MonoBehaviour
    {
        [SerializeField] private Image _skillImage;
        [SerializeField] private List<Image> _skillLevelIconObjectList;
        [SerializeField] private GameObject _activateSlotBGObject;
        [SerializeField] private GameObject _deActivateSlotBGObject;

        public void UpdateUI(bool isActivate, BaseSkillData activateSkillData = null, int currentLevel = 0)
        {
            _activateSlotBGObject.SetActive(isActivate);
            _deActivateSlotBGObject.SetActive(!isActivate);
            if (!isActivate)
            {
                return;
            }
            
            Sprite skillIconSprite = Manager.I.Resource.Load<Sprite>(activateSkillData.IconLabel);
            _skillImage.sprite = skillIconSprite;

            if (currentLevel == Const.MAX_AttackSKiLL_Level)
            {
                for (int i = 0; i < _skillLevelIconObjectList.Count; i++)
                {
                    Image image = _skillLevelIconObjectList[i];
                    image.gameObject.SetActive(i == 0);
                    //초월일 경우에는 빨간색
                    if (i == 0)
                    {
                        image.color = Const.UltimateSkillColor;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _skillLevelIconObjectList.Count; i++)
                {
                    Image image = _skillLevelIconObjectList[i];
                    image.gameObject.SetActive(true);
                    //비활성화 -> 회색, 아니면 색에 맞춰서
                    image.color = Utils.GetSkillTypeColor(activateSkillData.SkillType, i < currentLevel);
                }
            }
        }
    }
}