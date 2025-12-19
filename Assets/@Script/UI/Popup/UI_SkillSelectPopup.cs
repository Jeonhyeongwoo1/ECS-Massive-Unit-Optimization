using System;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Popup;
using MewVivor.Managers;
using MewVivor.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace MewVivor.Popup
{
    public class UI_SkillSelectPopup : BasePopup
    {
        [SerializeField] private List<UI_SkillCardItem> _skillCardItemList;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private List<Image> _activeSkillImageList;
        [SerializeField] private List<Image> _passiveSkillImageList;
        [SerializeField] private GameObject _adsObject;
        [SerializeField] private RectTransform _refreshTransform;
        [SerializeField] private GameObject _refreshObject;
        [SerializeField] private UI_SkillCardItem _potionCardItem;

        private Action<int> _onUpgradeOrAddNewSkillAction;
        
        public void AddEvent(Action onRefreshSkillListAction, Action<int> onUpgradeOrAddNewSkillAction)
        {
            _refreshButton.SafeAddButtonListener(onRefreshSkillListAction.Invoke);
            _onUpgradeOrAddNewSkillAction = onUpgradeOrAddNewSkillAction;
        }

        public void UpdateUI(List<BaseSkillData> recommendSkillList, List<Sprite> activeSkillSpriteList,
            List<Sprite> passiveSkillSpriteList, bool isFree, bool isPossibleRefresh)
        {
            _potionCardItem.Hide();
            if (recommendSkillList == null || recommendSkillList.Count == 0)
            {
                for (var i = 0; i < _skillCardItemList.Count; i++)
                {
                    _skillCardItemList[i].Hide();
                }
                
                _potionCardItem.SetSkillId(Const.ID_POTION);
                _potionCardItem.AddEvent(_onUpgradeOrAddNewSkillAction);
                _potionCardItem.gameObject.SetActive(true);
                _refreshObject.SetActive(isPossibleRefresh);
            }
            else
            {
                PlayerController player = Manager.I.Object.Player;
                for (var i = 0; i < _skillCardItemList.Count; i++)
                {
                    if (recommendSkillList.Count <= i)
                    {
                        _skillCardItemList[i].Hide();
                        break;
                    }

                    BaseSkillData skill = recommendSkillList[i];
                    bool isNewSkill = skill.CurrentLevel == 0;
                    Sprite sprite = null;
                    if (!string.IsNullOrEmpty(skill.IconLabel))
                    {
                        if (skill.SkillType == SkillType.AttackSkill &&
                            skill.CurrentLevel == Const.MAX_AttackSKiLL_Level - 1)
                        {
                            int skillId = skill.DataId + 5; //초월 시 스킬 이미지가 달라지므로 +1한다.
                            var skillData = Manager.I.Data.AttackSkillDict[skillId];
                            sprite = Manager.I.Resource.Load<Sprite>(skillData.IconLabel);
                        }
                        // else if (skill.SkillType == SkillType.PassiveSkill && skill.CurrentLevel == Const.MAX_PassiveSkill_Level - 1)
                        // {
                        //     int skillId = skill.DataId; //초월이 없기 떄문에 그대로
                        //     var skillData = Manager.I.Data.PassiveSkillDataDict[skillId];
                        //     sprite = Manager.I.Resource.Load<Sprite>(skillData.IconLabel);
                        // }
                        else
                        {
                            sprite = Manager.I.Resource.Load<Sprite>(skill.IconLabel);
                        }
                    }

                    DataManager dataManager = Manager.I.Data;
                    // 추천스킬에서 매칭 조합 표기 여부→ 새로운 스킬 + 내가 현재 가지고 있는 액티브 스킬이 있을 경우에만 패시브 스킬에 표기
                    Sprite matchSkillSprite = null;
                    bool isMatchedPassiveSkill = false;
                    if (skill.SkillType == SkillType.PassiveSkill)
                    {
                        bool isActivatedMatchPassiveSkill =
                            player.SkillBook.IsActivateMatchAttackSKill(skill.MatchSkillId);
                        if (isActivatedMatchPassiveSkill)
                        {
                            isMatchedPassiveSkill = true;
                        }

                        var matchSkillData = dataManager.AttackSkillDict[skill.MatchSkillId];
                        matchSkillSprite = Manager.I.Resource.Load<Sprite>(matchSkillData.IconLabel);
                    }

                    string title = dataManager.LocalizationDataDict[skill.TitleTextKey].GetValueByLanguage();
                    string description =
                        dataManager.LocalizationDataDict[skill.DescriptionTextKey].GetValueByLanguage();
                    int skillLevel = isNewSkill ? 1 : skill.CurrentLevel + 1;
                    _skillCardItemList[i].AddEvent(_onUpgradeOrAddNewSkillAction);
                    _skillCardItemList[i].UpdateUI(skill.DataId,
                        title,
                        sprite,
                        description,
                        isNewSkill,
                        skillLevel,
                        skill.SkillType,
                        matchSkillSprite,
                        isMatchedPassiveSkill);
                }

                foreach (var image in _activeSkillImageList)
                {
                    image.gameObject.SetActive(false);
                }

                foreach (var image in _passiveSkillImageList)
                {
                    image.gameObject.SetActive(false);
                }

                for (int i = 0; i < activeSkillSpriteList.Count; i++)
                {
                    _activeSkillImageList[i].sprite = activeSkillSpriteList[i];
                    _activeSkillImageList[i].gameObject.SetActive(true);
                }

                for (int i = 0; i < passiveSkillSpriteList.Count; i++)
                {
                    _passiveSkillImageList[i].sprite = passiveSkillSpriteList[i];
                    _passiveSkillImageList[i].gameObject.SetActive(true);
                }

                _refreshObject.SetActive(isPossibleRefresh);
                if (isFree)
                {
                    _adsObject.SetActive(false);
                    _refreshTransform.anchoredPosition = Vector2.zero;
                }
                else
                {
                    _adsObject.SetActive(true);
                    _refreshTransform.anchoredPosition = new Vector2(47.7f, 0);
                }
            }
        }
    }
}