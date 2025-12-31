using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Stat;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10141 : Projectile
    {
        [SerializeField] private GameObject _normalParticleObject;
        [SerializeField] private GameObject _ultimateEffectParticleObject;
        [SerializeField] private GameObject _spriteObject;
        
        private List<Transform> _onTriggerEnterTransformList = new List<Transform>();
        
        private readonly float _projectileSpeed = 10;
        private readonly float _heightArc = 3;
        private SkillCategoryType _skillCategoryType;
        private bool _isPossibleAttack;
        
        public override void Generate(Vector3 spawnPosition, 
                                                Vector3 targetPosition,
                                                AttackSkillData attackSkillData,
                                                CreatureController owner)
        {
            _isPossibleAttack = false;
            transform.position = owner.Position;
            gameObject.SetActive(true);

            float attackRange = attackSkillData.AttackRange * Utils.GetPlayerStat(CreatureStatType.CircleSkillSize);
            StartCoroutine(LaunchParabolaProjectile(spawnPosition, targetPosition, _projectileSpeed, _heightArc,
                () => ActivateParticle(attackRange, attackSkillData)));

            _skillCategoryType = attackSkillData.SkillCategoryType;
            StatModifer statModifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillDuration);
            float skillDuration = Utils.CalculateStatValue(attackSkillData.SkillDuration, statModifer);
            StartCoroutine(WaitDuration(skillDuration, Release));
            // StartCoroutine(ApplyDamageInterval(attackSkillData.AttackInterval));
        }

        private void ActivateParticle(float attackRange, AttackSkillData attackSkillData)
        {
            _isPossibleAttack = true;
            _spriteObject.SetActive(false);
            transform.eulerAngles = Vector3.zero;
            _normalParticleObject.SetActive(_skillCategoryType == SkillCategoryType.Normal);
            _ultimateEffectParticleObject.SetActive(_skillCategoryType == SkillCategoryType.Ultimate);

            // if (_skillCategoryType == SkillCategoryType.Normal)
            // {
            //     _normalParticleObject.transform.localScale = Vector3.zero;
            //     _normalParticleObject.transform.DOScale(Vector3.one * attackRange, 0.3f).SetEase(Ease.OutBack);
            // }
            // else
            // {
            //     _ultimateEffectParticleObject.transform.localScale = Vector3.zero;
            //     _ultimateEffectParticleObject.transform.DOScale(Vector3.one * attackRange, 0.3f).SetEase(Ease.OutBack);
            // }
            
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one * attackRange, 0.3f).SetEase(Ease.OutBack);
            CreateBaseSkillEntity(attackSkillData, true, attackSkillData.AttackInterval);
        }

        public override void Release()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(0, 0.3f))
                .OnComplete(() =>
                {
                    _spriteObject.SetActive(true);
                    _normalParticleObject.SetActive(false);
                    _ultimateEffectParticleObject.SetActive(false);
                    _normalParticleObject.transform.localScale = Vector3.one;
                    _ultimateEffectParticleObject.transform.localScale = Vector3.one;
                    transform.localScale = Vector3.one;
                    base.Release();
                });
        }
        
        // private IEnumerator ApplyDamageInterval(float interval)
        // {
        //     while (true)
        //     {
        //         foreach (Transform tr in _onTriggerEnterTransformList)
        //         {
        //             OnHit?.Invoke(tr, this);
        //         }
        //
        //         yield return new WaitForSeconds(interval);
        //     }
        // }
        
        // private void OnTriggerStay2D(Collider2D other)
        // {
        //     if (!_isPossibleAttack)
        //     {
        //         return;
        //     }
        //  
        //     if ( (other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox))
        //          &&!_onTriggerEnterTransformList.Contains(other.transform))
        //     {
        //         _onTriggerEnterTransformList.Add(other.transform);
        //         OnHit?.Invoke(other.transform, this);
        //     }
        // }
        //
        // protected override void OnTriggerExit2D(Collider2D other)
        // {
        //     if ((other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox))
        //         && _onTriggerEnterTransformList.Contains(other.transform))
        //     {
        //         _onTriggerEnterTransformList.Remove(other.transform);
        //     }
        // }
    }
}