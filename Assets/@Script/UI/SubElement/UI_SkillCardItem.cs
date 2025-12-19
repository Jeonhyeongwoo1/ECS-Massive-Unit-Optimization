using System;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.InGame.Popup
{
    public class UI_SkillCardItem : UI_SubItemElement
    {
        public int SkillId => _skillId;
        
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        [SerializeField] private GameObject _newSkillObject;
        [SerializeField] private Button _button;
        [SerializeField] private Image[] _skillLevelIndicatorArray;
        [SerializeField] private GameObject _matchingSkillInfoObject;
        [SerializeField] private Image _matchingSkillImage;
        [SerializeField] private GameObject _passiveSkillImageObject;
        [SerializeField] private GameObject _attackSkillImageObject;
        
        private int _skillId;
        
        public override void Initialize()
        {
            base.Initialize();
            
            gameObject.SetActive(true);
        }

        public void AddEvent(Action<int> onUpgradeOrAddNewSkillAction)
        {
            _button.SafeAddButtonListener(() => onUpgradeOrAddNewSkillAction?.Invoke(_skillId));
        }

        public void SetSkillId(int skillId)
        {
            _skillId = skillId;
        }

        public void UpdateUI(int skillId, string skillName, Sprite skillSprite, string skillDescription,
            bool isNewSkill, int skillLevel, SkillType skillType, Sprite matchSkillSprite, bool isMatchedPassiveSkill)
        {
            _skillId = skillId;
            _skillNameText.text = skillName;
            // _skillNameText.text = skillId.ToString();
            _skillImage.sprite = skillSprite;
            _skillDescriptionText.text = skillDescription;
            _newSkillObject.SetActive(isNewSkill);

            Sprite iconSprite = null;
            if (skillType == SkillType.AttackSkill)
            {
                iconSprite = Manager.I.Resource.Load<Sprite>("ActiveSkill_Icon");
            }
            else
            {
                iconSprite = Manager.I.Resource.Load<Sprite>("PassiveSKill_Icon");
            }
            
            _passiveSkillImageObject.gameObject.SetActive(skillType != SkillType.AttackSkill);
            _attackSkillImageObject.gameObject.SetActive(skillType == SkillType.AttackSkill);

            _matchingSkillInfoObject.SetActive(matchSkillSprite != null);
            _matchingSkillImage.color = Color.white;
            _matchingSkillImage.sprite = matchSkillSprite;
            _matchingSkillImage.DOKill();
            
            if (isMatchedPassiveSkill)
            { 
                _matchingSkillImage.DOFade(0f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine).
                    SetUpdate(true);
            }

            if (skillLevel == Const.MAX_AttackSKiLL_Level)
            {
                for (var i = 0; i < _skillLevelIndicatorArray.Length; i++)
                {
                    if (i == 0)
                    {
                        _skillLevelIndicatorArray[i].gameObject.SetActive(true);
                        Image image = _skillLevelIndicatorArray[i];
                        Color color = Const.UltimateSkillColor;
                        image.sprite = iconSprite;
                        image.color = color;
                        image.DOKill();
                        image.DOFade(0f, 0.5f)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine).
                            SetUpdate(true);
                    }
                    else
                    {
                        _skillLevelIndicatorArray[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (var i = 0; i < _skillLevelIndicatorArray.Length; i++)
                {
                    Image image = _skillLevelIndicatorArray[i];
                    Color color;
                    if (i < skillLevel)
                    {
                        color = skillType == SkillType.AttackSkill
                            ? Const.ActivateAttackSkillColor
                            : Const.ActivatePassiveSkillColor;
                    }
                    else
                    {
                        color = skillType == SkillType.AttackSkill
                            ? Const.DeActivateAttackSkillColor
                            : Const.DeActivatePassiveSkillColor;
                    }
                
                    image.sprite = iconSprite;
                    image.color = color;
                    image.DOKill();
                    if (i == skillLevel - 1)
                    {
                        image.DOFade(0f, 0.5f)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine).
                            SetUpdate(true);
                    }
                    
                    image.gameObject.SetActive(true);
                }
            }
        }

        private void OnDisable()
        {
            _matchingSkillImage.DOKill();
            foreach (var image in _skillLevelIndicatorArray)
            {
                image.DOKill();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}