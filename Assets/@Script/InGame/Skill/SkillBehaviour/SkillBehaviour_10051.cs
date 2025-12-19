using System;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10051 : Projectile
    {
        [SerializeField] private GameObject _normalProjectileObject;
        [SerializeField] private GameObject _normalProjectileParticleObject;
        [SerializeField] private GameObject _ultimateProjectileParticleObject;
        
        private CreatureController _owner;
        private AttackSkillData _attackSkillData;
        private Collider2D[] _collider2Ds = new Collider2D[100];
        private bool _isMaxLevel;
        
        public override void Generate(Transform targetTransform, Vector3 direction, AttackSkillData attackSkillData, CreatureController owner, int currentLevel)
        {
            transform.position = targetTransform.position;
            transform.localScale = Vector3.one * attackSkillData.Scale;
            _owner = owner;
            _attackSkillData = attackSkillData;

            bool isMaxLevel = Const.MAX_AttackSKiLL_Level == currentLevel;
            _isMaxLevel = isMaxLevel;
            _normalProjectileObject.SetActive(true);
            _normalProjectileParticleObject.SetActive(false);
            _ultimateProjectileParticleObject.SetActive(false);
            _rigidbody.simulated = true;
            gameObject.SetActive(true);
            
            var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.ProjectileSpeed);
            float speed = Utils.CalculateStatValue(attackSkillData.ProjectileSpeed, modifer);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rigidbody.SetRotation(angle);
            _rigidbody.linearVelocity = direction * speed;

            StartCoroutine(WaitDuration(Const.PROJECTILE_LIFE_CYCLE, Release));
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Monster) && !other.CompareTag(Tag.ItemBox))
            {
                return;
            }
            
            OnHit.Invoke(other.transform, this);
            var modifer = _owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.ExplsionRange);
            float attackRange = Utils.CalculateStatValue(_attackSkillData.AttackRange, modifer);
            float explosionSkillSize = Utils.GetPlayerStat(CreatureStatType.ExplosionSkillSize);
            attackRange *= explosionSkillSize;
            
            int cnt = Physics2D.OverlapCircleNonAlloc(transform.position, attackRange,
                _collider2Ds, Layer.AttackableLayer);
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    Collider2D col = _collider2Ds[i];
                    if (col == null || other == col)
                    {
                        continue;
                    }

                    OnHit.Invoke(col.transform, this);
                }
            }
            
            _normalProjectileParticleObject.SetActive(!_isMaxLevel);
            _ultimateProjectileParticleObject.SetActive(_isMaxLevel);
            _normalProjectileObject.SetActive(false);
            _rigidbody.simulated = false;
            DOVirtual.DelayedCall(1, Release);
        }
    }
}