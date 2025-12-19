using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Skill;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public partial class PlayerController
    {
        public void Cheat_LearnAttackSkill(AttackSkillType skillId)
        {
            AttackSkillType attackSkillType = skillId;
            BaseAttackSkill attackSkill = _skillBook.ActivateAttackSkillList.Find(v => v.AttackSkillType == attackSkillType);
            int id = 0;
            if (attackSkill != null)
            {
                id = attackSkill.AttackSkillData.DataId;
            }
            else
            {
                AttackSkillData skillData = Manager.I.Data.AttackSkillDict[(int)skillId];
                id = skillData.DataId;
            }
            
            OnUpgradeOrAddNewSkill(id, true);
        }

        public bool Cheat_RemoveAttackSkill(AttackSkillType skillType)
        {
            AttackSkillType attackSkillType = skillType;
            BaseAttackSkill attackSkill = _skillBook.ActivateAttackSkillList.Find(v => v.AttackSkillType == attackSkillType);
            int id = 0;
            if (attackSkill != null)
            {
                id = attackSkill.AttackSkillData.DataId;
            }
            else
            {
                AttackSkillData skillData = Manager.I.Data.AttackSkillDict[(int)skillType];
                id = skillData.DataId;
            }

            return RemoveAttackSkill(id);
        }

        public void Cheat_UpdateAttackSkillData(AttackSkillData attackSkillData)
        {
            BaseAttackSkill skill =
                _skillBook.ActivateAttackSkillList.Find(v => v.AttackSkillType == attackSkillData.AttackSkillType);
            
            if(skill == null)
            {
                Debug.LogError("Skill is null");
                return;
            }

            skill.UpdateSkillData(attackSkillData);
        }

        public void Cheat_LearnPassiveSkill(PassiveSkillType passiveSkillType)
        {
            BasePassiveSkill skill =
                _skillBook.ActivatePassiveSkillList.Find(v => v.PassiveSkillType == passiveSkillType);
            int id = 0;
            if (skill != null)
            {
                id = skill.PassiveSkillData.DataId;
            }
            else
            {
                PassiveSkillData skillData = Manager.I.Data.PassiveSkillDataDict[(int)passiveSkillType];
                id = skillData.DataId;
            }

            OnUpgradeOrAddNewSkill(id, true);
        }
    }
}