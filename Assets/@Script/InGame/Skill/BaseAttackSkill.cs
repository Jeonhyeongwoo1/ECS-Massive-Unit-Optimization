using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public abstract class BaseAttackSkill : BaseSkill
    {
        public AttackSkillData AttackSkillData => _skillData as AttackSkillData;
        public AttackSkillType AttackSkillType => AttackSkillData.AttackSkillType;
        public float AccumulatedDamage { get; protected set; }
        protected virtual string HitSoundName { get; set; }
        
        public virtual void Initialize(CreatureController owner, AttackSkillData attackSkillData)
        {
            _owner = owner;
            _skillData = attackSkillData;
            _skillData.CurrentLevel = 1;
            _currentLevel = 1;
            
            if (_owner.CreatureType == CreatureType.Player)
            {
                // Manager.I.Event.Raise(GameEventType.LearnSkill, _skillData);
            }
        }
        
        public abstract UniTask StartSkillLogicProcessAsync();
        public abstract void StopSkillLogic();
        protected abstract UniTask UseSkill();

        public virtual void OnChangedSkillData() { }
        
        protected virtual void OnHit(Transform target, Projectile projectile)
        {
            Hit(target, projectile);
        }

        protected virtual void Hit(Transform target, Projectile projectile)
        {
            if (Utils.TryGetComponentInParent(target.gameObject, out IHitable hitable))
            {
                if (hitable is MonsterController)
                {
                    if (AttackSkillData.KnockbackDistance != 0)
                    {
                        var monster = hitable as MonsterController;
                        monster.ExecuteKnockback(AttackSkillData.KnockbackDistance);
                    }
                
                    if (AttackSkillData.DebuffType1 != DebuffType.None)
                    {
                        var monster = hitable as MonsterController;
                        monster.ApplyDebuff(AttackSkillData.DebuffType1, AttackSkillData.DebuffValue1,
                            AttackSkillData.DebuffValuePercent1, AttackSkillData.DebuffDuration1);
                    }

                    if (AttackSkillData.DebuffType2 != DebuffType.None)
                    {
                        var monster = hitable as MonsterController;
                        monster.ApplyDebuff(AttackSkillData.DebuffType2, AttackSkillData.DebuffValue2,
                            AttackSkillData.DebuffValuePercent2, AttackSkillData.DebuffDuration2);
                    }
                }
                
                float damage = GetDamage();
                hitable.TakeDamage(damage, _owner);
                AccumulatedDamage += damage;
                // Manager.I.Audio.Play(Sound.Effect, HitSoundName);
            }
        }

        protected virtual float GetDamage()
        {
            float damage = _owner.Atk.Value;
            //0 ~ 1
            if (AttackSkillData.DamagePercent > 0)
            {
                damage *= AttackSkillData.DamagePercent;
            }

            return damage;
        }

        protected virtual void OnExit(Transform target, Projectile projectile)
        {
            
        }

        public override void UpdateSkillData(BaseSkillData skillData)
        {
            _skillData = skillData;
        }
    }
}