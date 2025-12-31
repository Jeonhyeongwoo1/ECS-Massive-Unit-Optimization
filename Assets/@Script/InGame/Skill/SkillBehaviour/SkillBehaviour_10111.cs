using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10111 : Projectile
    {
        private Collider2D[] _collider2Ds = new Collider2D[100];

        [SerializeField] private List<TriggerHitComponent> _skillObjectList;

        private float _skillRange;
        private int _monsterCount;
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, AttackSkillData attackSkillData,
            CreatureController owner)
        {
            int cnt = attackSkillData.NumOfProjectile;
            List<Vector3> monsterPositionList = Manager.I.Object.GetCenterMonsterEntityPositionInCameraArea(cnt);
            _monsterCount = monsterPositionList == null ? 0 : monsterPositionList.Count;
            if (monsterPositionList == null)
            {
                Vector3 position = Manager.I.Object.GetRandomPositionInCameraArea();
                position.z = 0;
                transform.localScale = Vector3.one * attackSkillData.Scale;
                gameObject.SetActive(true);
                TriggerHitComponent skillTransform = _skillObjectList[0];
                skillTransform.gameObject.SetActive(true);
                skillTransform.transform.position = position;
                StartCoroutine(AttackCor(owner, attackSkillData, skillTransform));
            }
            else
            {
                transform.localScale = Vector3.one * attackSkillData.Scale;
                gameObject.SetActive(true);

                for (var i = 0; i < monsterPositionList.Count; i++)
                {
                    TriggerHitComponent skillTransform = _skillObjectList[i];
                    skillTransform.gameObject.SetActive(true);
                    skillTransform.transform.position = monsterPositionList[i];
                    StartCoroutine(AttackCor(owner, attackSkillData, skillTransform));
                }    
            }
        }

        private IEnumerator AttackCor(CreatureController owner, AttackSkillData attackSkillData, TriggerHitComponent skillTransform)
        {   
            yield return new WaitForSeconds(0.4f);
            var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillRange);
            float skillRange = Utils.CalculateStatValue(attackSkillData.AttackRange, modifer);
            skillRange *= Utils.GetPlayerStat(CreatureStatType.ExplosionSkillSize);

            var skillEntity = CreateBaseSkillEntity(skillTransform, attackSkillData);
            CreateExplosionSkillComponent(skillEntity, skillRange);
            
            // int cnt = Physics2D.OverlapCircleNonAlloc(skillTransform.position, skillRange,
            //     _collider2Ds, Layer.AttackableLayer);
            //
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

            _skillRange = skillRange;
        }
        
        public override void Release()
        {
            foreach (TriggerHitComponent go in _skillObjectList)
            {
                go.gameObject.SetActive(false);
            }
            
            base.Release();
        }

        private void OnDrawGizmos()
        {
            // if (_skillObjectList == null || _skillObjectList.Count == 0)
            //     return;
            //
            // Gizmos.color = Color.red;
            // for (int j = 0; j < _monsterCount; j++)
            // {
            //     Vector3 center = _skillObjectList[j].position;
            //     float angleStep = 360f / 30;
            //     Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0), 0) * _skillRange;
            //
            //     for (int i = 1; i <= 30; i++)
            //     {
            //         float angle = angleStep * i * Mathf.Deg2Rad;
            //         Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _skillRange;
            //         Gizmos.DrawLine(prevPoint, nextPoint);
            //         prevPoint = nextPoint;
            //     }   
            // }
        }
    }
}