using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Stat;
using MewVivor.Key;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor.InGame.Skill
{
    /// <summary>
    ///  스킬 설명 :  주기적으로 벽에 튕기는 털실을 굴려, 털실에 닿은 적들에게 충돌 데미지를 입힘.
    /// </summary>
    public class Skill_10021 : RepeatAttackSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        protected override async UniTask UseSkill()
        {
            int count = AttackSkillData.NumOfProjectile;
            for (int i = 0; i < count; i++)
            {
                float radius = 5;
                float angle = (360f / count) * i * Mathf.Deg2Rad; // 각도 분배
                float randomValue = Random.Range(0, 15);
                angle += randomValue;
                if (angle > 360)
                {
                    angle -= 360;
                }

                Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10021);
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                Vector3 position = new Vector3(x, y) + _owner.Position;
                Vector3 direction = (position - _owner.Position).normalized;
                GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                var generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.Generate(_owner.transform, direction, AttackSkillData, _owner, CurrentLevel);

                _projectileList.Add(generatable);
                try
                {
                    await UniTask.WaitForSeconds(AttackSkillData.ProjectileSpacing, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                    StopSkillLogic();
                    break;
                }
            }

            StatModifer statModifer = _owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillDuration);
            float skillDuration = Utils.CalculateStatValue(AttackSkillData.SkillDuration, statModifer);
            try
            {
                await UniTask.WaitForSeconds(skillDuration, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
                return;
            }
            
            Release();
        }
    }
}