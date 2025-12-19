using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.Key;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor.InGame.Skill
{
    /// <summary>
    ///  스킬 설명 :  주기적으로 생선뼈를 직선으로 날려 닿은 적들에게 데미지를 입힘
    ///             벽에 부딪치면 사라짐
    /// </summary>
    public class Skill_10031 : RepeatAttackSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        protected override async UniTask UseSkill()
        {
            int count = AttackSkillData.NumOfProjectile;
            int index = 0;
            List<MonsterController> list = Manager.I.Object.GetNearestMonsterList(count);

            if (list != null)
            {
                foreach (MonsterController monster in list)
                {
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10031);
                    index++;
                    //한개인 경우에는 플레이어가 바라보는 방향으로 발사
                    Vector3 direction = (monster.Position - _owner.Position).normalized;
                    GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    generatable.Generate(_owner.transform, direction, AttackSkillData, _owner, CurrentLevel);
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
                
                int remainCount =  count - index;
                if (remainCount > 0)
                {
                    for (int i = 0; i < remainCount; i++)
                    {
                        Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10031);
                        //한개인 경우에는 플레이어가 바라보는 방향으로 발사
                        Vector3 position = Manager.I.Object.GetRandomPositionOutsideCamera();
                        Vector3 direction = (position - _owner.Position).normalized;
                        GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                        var generatable = prefab.GetComponent<IGeneratable>();
                        generatable.OnHit = OnHit;
                        generatable.Generate(_owner.transform, direction, AttackSkillData, _owner, CurrentLevel);
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
            }
        }
    }
}