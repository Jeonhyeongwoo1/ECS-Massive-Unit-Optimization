using MewVivor;
using MewVivor.Enum;
using MewVivor.InGame.Popup;
using MewVivor.InGame.Skill;
using MewVivor.Managers;
using MewVivor.Popup;
using UnityEngine;

namespace MewVivor.Popup
{
    public class UI_LearnSkillPopup : BasePopup
    {
        [SerializeField] private Transform _skillCardParentTransform;

        private UI_SkillCardItem _skillItemCard;
        
        public override void Initialize()
        {
            base.Initialize();

            SafeButtonAddListener(ref _bgCloseButton,
                () => Manager.I.Event.Raise(GameEventType.UpgradeOrAddNewSkill, _skillItemCard.SkillId));
            
            _skillItemCard = Manager.I.UI.AddSubElementItem<UI_SkillCardItem>(_skillCardParentTransform);
            _skillItemCard.Initialize();
        }

        public void UpdateSKillItem(BaseAttackSkill recommendAttackSkill)
        {
            // BaseAttackSkill attackSkill = recommendAttackSkill;
            // Sprite sprite = Manager.I.Resource.Load<Sprite>(attackSkill.AttackSkillData.IconLabel);
            // _skillItemCard.UpdateUI(attackSkill.AttackSkillData.DataId, attackSkill.AttackSkillData.Name, sprite,
            //     attackSkill.AttackSkillData.Description,
            //     false, attackSkill.CurrentLevel);
        }
    }
}