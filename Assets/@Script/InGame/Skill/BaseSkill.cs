using MewVivor.Data;
using MewVivor.Enum;

namespace MewVivor.InGame.Skill
{
    public abstract class BaseSkill
    {
        public int CurrentLevel => _currentLevel;

        public bool IsMaxLevel
        {
            get
            {
                if (SkillType == SkillType.AttackSkill)
                {
                    return _currentLevel == Const.MAX_AttackSKiLL_Level;
                }
                else
                {
                    return _currentLevel == Const.MAX_PassiveSkill_Level;
                }
            }
        }

        public SkillType SkillType => _skillData.SkillType;
        public BaseSkillData SkillData => _skillData;
        
        protected CreatureController _owner;
        protected BaseSkillData _skillData;
        protected int _currentLevel;

        public virtual void UpdateSkillData(BaseSkillData skillData) {}
        
        public virtual void LevelUp(BaseSkillData skillData)
        {
            _skillData = skillData;
            _currentLevel++;
        }
    }
}