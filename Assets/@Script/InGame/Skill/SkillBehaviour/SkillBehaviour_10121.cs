using System;
using System.Collections;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10121 : Projectile
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CircleCollider2D _trigger;
        
        private Collider2D[] _collider2Ds = new Collider2D[100];
        private SkillCategoryType _skillCategoryType;
        private float _skillRange;

        private static class Anim
        {
            public static int Normal = Animator.StringToHash("Skill");
            public static int Ultimate = Animator.StringToHash("Ultimate");
        }
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, AttackSkillData attackSkillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            _trigger.gameObject.SetActive(false);

            _animator.Play(Anim.Normal);
            _skillCategoryType = attackSkillData.SkillCategoryType;
            StartCoroutine(AttackCor(owner, attackSkillData, transform));
        }
        
        private IEnumerator AttackCor(CreatureController owner, AttackSkillData attackSkillData, Transform skillTransform)
        {   
            yield return new WaitForSeconds(0.2f);
            var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillRange);
            float skillRange = Utils.CalculateStatValue(attackSkillData.AttackRange, modifer);
            skillRange *= Utils.GetPlayerStat(CreatureStatType.ExplosionSkillSize);

            var skillEntity = CreateBaseSkillEntity(attackSkillData);
            CreateExplosionSkillComponent(skillEntity,  skillRange);
            
            // int cnt = Physics2D.OverlapCircleNonAlloc(skillTransform.position, skillRange,
            //     _collider2Ds, Layer.AttackableLayer);
            //
            // _skillRange = skillRange;
            // if (cnt > 0)
            // {
            //     for (int i = 0; i < cnt; i++)
            //     {
            //         Collider2D col = _collider2Ds[i];
            //         if (col == null)
            //         {
            //             continue;
            //         }
            //
            //         OnHit?.Invoke(col.transform, this);
            //     }
            // }

            if (attackSkillData.SkillCategoryType == SkillCategoryType.Normal)
            {
                yield break;
            }

            Camera cam = Camera.main;
            _trigger.gameObject.SetActive(true);
            _animator.Play(Anim.Ultimate);
            Vector3 worldToViewport = cam.WorldToViewportPoint(transform.position);
            float x = Mathf.Max((1 - worldToViewport.x), worldToViewport.x);
            float y = Mathf.Max((1 - worldToViewport.y), worldToViewport.y);

            Vector3 direction;
            if (x <= y)
            {
                direction = worldToViewport.x < 0.5f ? Vector3.right : Vector3.left;
            }
            else
            {
                direction = worldToViewport.y < 0.5f ? Vector3.up : Vector3.down;
            }

            //TODO : 추후에 스킬의 스피드로 변경해야함
            // _rigidbody.linearVelocity = direction * 20f;

            //새롭게 생성
            skillEntity = CreateBaseSkillEntity(attackSkillData);
            float elapsed = 0;
            float bookReleaseDuration = 3;
            while (true)
            {
                elapsed += Time.deltaTime;
                worldToViewport = cam.WorldToViewportPoint(transform.position);
                if (worldToViewport.x > 1 || worldToViewport.y > 1)
                {
                    break;
                }

                if (elapsed > bookReleaseDuration)
                {
                    break;
                }

                transform.Translate(direction * (20 * Time.deltaTime));
                yield return null;
            }
            
            Release();
        }

        // protected override void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (_skillCategoryType == SkillCategoryType.Ultimate)
        //     {
        //         base.OnTriggerEnter2D(other);
        //     }
        // }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 center = transform.position;
            float angleStep = 360f / 30;
            Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * _skillRange;

            for (int i = 1; i <= 30; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _skillRange;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }   
        }
    }
}