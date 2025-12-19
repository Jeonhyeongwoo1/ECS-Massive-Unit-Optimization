using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Stat;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class Skill_10131 : RepeatAttackSkill
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
                    index++;
                    //한개인 경우에는 플레이어가 바라보는 방향으로 발사
                    Vector3 direction = (monster.Position - _owner.Position).normalized;
                    GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    generatable.Generate(_owner.transform.position, direction, AttackSkillData, _owner);
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
            
            StatModifer statModifer = _owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillDuration);
            float skillDuration = Utils.CalculateStatValue(AttackSkillData.SkillDuration, statModifer);
            try
            {
                await UniTask.WaitForSeconds(skillDuration, 
                    cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"error {nameof(UseSkill)} / {e.Message}");
                return;
            }
        }
    }
}