using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Stat;

namespace MewVivor.InGame.Skill
{
    public class BasePassiveSkill : BaseSkill
    {
        public PassiveSkillType PassiveSkillType => PassiveSkillData.PassiveSkillType;
        public PassiveSkillData PassiveSkillData => _skillData as PassiveSkillData;
        public StatModifer PassiveStatModifer => _passiveStatModifer;
        
        private StatModifer _passiveStatModifer;
        private CreatureStat _creatureStat;
        
        public void Initialize(PassiveSkillData passiveSkillData)
        {
            _skillData = passiveSkillData;
            // _creatureStat = creatureStat;
            _skillData.CurrentLevel = 1;
            _currentLevel = 1;
            _passiveStatModifer = new StatModifer(PassiveSkillData.Rate * CurrentLevel, ModifyType.PercentAdd, this);
        }

        public (PassiveSkillType, StatModifer) GetStat()
        {
            return (PassiveSkillType, _passiveStatModifer);
        }

        public override void LevelUp(BaseSkillData skillData)
        {
            base.LevelUp(skillData);
            _passiveStatModifer.Value = PassiveSkillData.Rate * CurrentLevel;
        }

        public bool IsLinkedSkill(int skillId)
        {
            return PassiveSkillData.LinkedSkillId.Contains(skillId);
        }
    }
}