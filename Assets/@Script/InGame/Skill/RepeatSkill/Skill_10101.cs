using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Stat;
using MewVivor.Key;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor.InGame.Skill
{
    public class Skill_10101 : RepeatAttackSkill
    {
        protected override async UniTask UseSkill()
        {
            GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
            var generatable = prefab.GetComponent<IGeneratable>();
            generatable.OnHit = OnHit;
            Vector3 spawnPosition = _owner.Position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
            Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10101);
            generatable.Generate(spawnPosition, Vector3.zero, AttackSkillData, _owner);
        }

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }
        
        protected override void OnExit(Transform target, Projectile projectile)
        {
            if (Utils.TryGetComponentInParent(target.gameObject, out CreatureController creature))
            {
                if (AttackSkillData.DebuffType1 != DebuffType.None)
                {
                    var monster = creature as MonsterController;
                    monster.RemoveBuff(AttackSkillData.DebuffType1);
                }

                if (AttackSkillData.DebuffType2 != DebuffType.None)
                {
                    var monster = creature as MonsterController;
                    monster.RemoveBuff(AttackSkillData.DebuffType2);
                }
            }
        }
    }
}