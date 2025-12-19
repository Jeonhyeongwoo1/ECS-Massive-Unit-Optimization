using MewVivor.Common;
using MewVivor.Data;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Skill.SKillBehaviour;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class EquipmentSkill
    {
        protected CreatureController _owner;
        protected EquipmentSkillData _equipmentSkillData;

        public virtual void Initialize(CreatureController owner, EquipmentSkillData equipmentSkillData)
        {
            _owner = owner;
            _equipmentSkillData = equipmentSkillData;
        }

        protected virtual void OnHit(Transform target, EquipmentSkillProjectile projectile)
        {
            if (Utils.TryGetComponentInParent(target.gameObject, out IHitable hitable))
            {
                if (hitable is not MonsterController)
                {
                    return;
                }

                float damage = GetDamage();
                hitable.TakeDamage(damage, _owner);
            }
        }

        protected virtual float GetDamage()
        {
            return 0;
        }
    }
}