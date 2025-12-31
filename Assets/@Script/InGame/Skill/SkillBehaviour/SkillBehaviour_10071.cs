using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10071 : Projectile
    {
        [SerializeField] private ParticleSystem _normalParticle;
        [SerializeField] private Material _noramlMaterial;
        [SerializeField] private Material _ultimateMaterial;
        private List<Transform> _onTriggerEnterTransformList = new();
        private float _skillRange;
        private Unity.Entities.Entity _skillEntity;
        
        public override void Generate(Transform targetTransform, Vector3 direction, AttackSkillData attackSkillData, CreatureController owner, int currentLevel)
        {
            transform.SetParent(owner.transform);
            
            var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillRange);
            float skillRange = Utils.CalculateStatValue(attackSkillData.AttackRange, modifer);
            skillRange *= Utils.GetPlayerStat(CreatureStatType.CircleSkillSize);
            _skillRange = skillRange;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one * skillRange;

            _normalParticle.gameObject.SetActive(true);
            ParticleSystem.MainModule mainModule = _normalParticle.main;
            mainModule.startSize = 0.7f * skillRange;
            var renderer = _normalParticle.GetComponent<ParticleSystemRenderer>();
            renderer.material = currentLevel == Const.MAX_AttackSKiLL_Level ? _ultimateMaterial : _noramlMaterial;
            gameObject.SetActive(true);

            _skillEntity = CreateBaseSkillEntity(attackSkillData, true, attackSkillData.AttackInterval);
            // StartCoroutine(ApplyDamageInterval(attackSkillData.AttackInterval));
        }
        
        // protected override void OnTriggerEnter2D(Collider2D other)
        // {
        //     if ((other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox)) &&
        //         !_onTriggerEnterTransformList.Contains(other.transform))
        //     {
        //         _onTriggerEnterTransformList.Add(other.transform);
        //         OnHit?.Invoke(other.transform, this);
        //     }
        // }
        //
        // protected override void OnTriggerExit2D(Collider2D other)
        // {
        //     if ((other.CompareTag(Tag.Monster) || other.CompareTag(Tag.ItemBox)) &&
        //         _onTriggerEnterTransformList.Contains(other.transform))
        //     {
        //         _onTriggerEnterTransformList.Remove(other.transform);
        //     }
        // }
   
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

        public override void Release()
        {
            base.Release();
            
            DestroySkillEntity(_skillEntity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 center = transform.position;
            float angleStep = 360f / 30;
            Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * _skillRange * 0.5f;

            for (int i = 1; i <= 30; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _skillRange * 0.5f;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }  
        }
    }
}