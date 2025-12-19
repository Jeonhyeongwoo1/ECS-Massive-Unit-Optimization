using System;
using System.Collections;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Stat;
using MewVivor.InGame.Enum;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10091 : Projectile
    {
        [SerializeField] private GameObject _particleObject;
        [SerializeField] private GameObject _spriteObject;
        [SerializeField] private CircleCollider2D _collider2D;

        private List<Transform> _onTriggerEnterTransformList = new();
        private readonly float _projectileSpeed = 10;
        private readonly float _heightArc = 3;
        private readonly float _defaultColliderRadius = 3f;
        private bool _isPossibleAttack;
        private AudioSource _audio;
        
        public override void Generate(Vector3 spawnPosition, Vector3 targetPosition, AttackSkillData attackSkillData,
            CreatureController owner)
        {
            _isPossibleAttack = false;
            transform.position = owner.transform.position;

            float range = attackSkillData.Scale * Utils.GetPlayerStat(CreatureStatType.CircleSkillSize);
            _collider2D.radius = _defaultColliderRadius * range;
            _particleObject.transform.localScale = Vector3.one * range;

            Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10091, 0.5f, 0.25f);
            gameObject.SetActive(true);
            StartCoroutine(LaunchParabolaProjectile(transform.position, targetPosition, _projectileSpeed,
                _heightArc, () => ActivateParticle(owner, attackSkillData)));
        }

        private async void ActivateParticle(CreatureController owner, AttackSkillData attackSkillData)
        {
            _spriteObject.SetActive(false);
            _particleObject.SetActive(true);
            _isPossibleAttack = true;
            transform.eulerAngles = Vector3.zero;
            StatModifer statModifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillDuration);
            float skillDuration = Utils.CalculateStatValue(attackSkillData.SkillDuration, statModifer);
            StartCoroutine(WaitDuration(skillDuration, Release));
            StartCoroutine(ApplyDamageInterval(attackSkillData.AttackInterval));
            _audio = await Manager.I.Audio.Play(Sound.SFX, SoundKey.UsingSkill_10091,0.5f, 0.25f);
        }

        public override void Release()
        {
            base.Release();

            if (_audio != null)
            {
                _audio.Stop();
            }
            
            _spriteObject.SetActive(true);
            _particleObject.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!_isPossibleAttack)
            {
                return;
            }
            if ( (other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox))
                    &&!_onTriggerEnterTransformList.Contains(other.transform))
            {
                _onTriggerEnterTransformList.Add(other.transform);
                OnHit?.Invoke(other.transform, this);
            }
        }
        
        protected override void OnTriggerExit2D(Collider2D other)
        {
            if ((other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox))
                && _onTriggerEnterTransformList.Contains(other.transform))
            {
                _onTriggerEnterTransformList.Remove(other.transform);
            }
        }
        
        private IEnumerator ApplyDamageInterval(float interval)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);

                foreach (Transform tr in _onTriggerEnterTransformList)
                {
                    OnHit?.Invoke(tr, this);
                }
            }
        }
    }
}