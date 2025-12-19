using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Stat;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10101 : Projectile
    {
        [SerializeField] private Animator _animator;
        
        private static class Anim
        {
            public static int SkillStart = Animator.StringToHash("Skill_Start");
            public static int SkillEnd = Animator.StringToHash("Skill_End");
        }
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, AttackSkillData attackSkillData,
            CreatureController owner)
        {
            var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillRange);
            float skillRange = Utils.CalculateStatValue(attackSkillData.AttackRange, modifer);
            skillRange *= Utils.GetPlayerStat(CreatureStatType.CircleSkillSize);
            transform.localScale = Vector3.one * skillRange;
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            _animator.Play(Anim.SkillStart);
            StatModifer statModifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillDuration);
            float skillDuration = Utils.CalculateStatValue(attackSkillData.SkillDuration, statModifer);
            StartCoroutine(WaitDuration(skillDuration, Release));
        }

        public override void Release()
        {
            _animator.Play(Anim.SkillEnd);
            DOVirtual.DelayedCall(0.3f, () => base.Release());
        }
    }
}