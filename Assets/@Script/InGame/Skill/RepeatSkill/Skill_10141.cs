using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Skill;
using MewVivor.Key;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor.InGame.Skill
{
    public class Skill_10141 : RepeatAttackSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }
        
        protected override async UniTask UseSkill()
        {
            float random = Random.Range(0, 360);
            //랜덤한 곳에만 던지기
            int count = AttackSkillData.NumOfProjectile;
            for (int i = 0; i < count; i++)
            {
                float radius = Random.Range(5, 10);
                float angle = (360f / count) * i * Mathf.Deg2Rad + random; // 각도 분배
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                Vector3 position = new Vector3(x, y) + _owner.Position;
                GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                var generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.OnExit = OnExit;
                generatable.Generate(_owner.Position, position, AttackSkillData, _owner);

                Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10141, 0.5f, 0.25f);
                try
                {
                    await UniTask.WaitForSeconds(AttackSkillData.ProjectileSpacing, cancelImmediately: true);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                    StopSkillLogic();
                    break;
                }
            }
        }
        
        // protected override void OnExit(Transform target, Projectile projectile)
        // {
        //     if (Utils.TryGetComponentInParent(target.gameObject, out CreatureController creature))
        //     {
        //         if (AttackSkillData.DebuffType1 != DebuffType.None)
        //         {
        //             var monster = creature as MonsterController;
        //             monster.RemoveBuff(AttackSkillData.DebuffType1);
        //         }
        //
        //         if (AttackSkillData.DebuffType2 != DebuffType.None)
        //         {
        //             var monster = creature as MonsterController;
        //             monster.RemoveBuff(AttackSkillData.DebuffType2);
        //         }
        //     }
        // }
    }
}