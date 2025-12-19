using MewVivor.Data;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Skill.SKillBehaviour;
using MewVivor.InGame.Stat;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class AuraSkill : EquipmentSkill
    {
        private EquipmentSkillProjectile _equipmentSkillProjectile;

        public override void Initialize(CreatureController owner, EquipmentSkillData equipmentSkillData)
        {
            base.Initialize(owner, equipmentSkillData);

            GameObject prefab = Manager.I.Resource.Instantiate("AuraSkill");
            _equipmentSkillProjectile = prefab.GetComponent<EquipmentSkillProjectile>();
            _equipmentSkillProjectile.OnHit = OnHit;
            _equipmentSkillProjectile.Generate(owner, equipmentSkillData);
        }

        protected override float GetDamage()
        {
            var player = _owner as PlayerController;
            CreatureStat auraDamageUpPercent = player.AuraDamageUpPercent;
            float value = auraDamageUpPercent.Value;
            float damagePercent = _equipmentSkillData.Type_Value1;
            float finalPercent = damagePercent + value;
            float damage = _owner.Atk.Value * finalPercent;
            return damage;
        }
    }
}