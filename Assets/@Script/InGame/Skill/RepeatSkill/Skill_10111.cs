using System;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class Skill_10111 : RepeatAttackSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        protected override async UniTask UseSkill()
        {
            GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
            var generatable = prefab.GetComponent<IGeneratable>();
            generatable.OnHit = OnHit;
            Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10111);
            generatable.Generate(_owner.Position, Vector3.zero, AttackSkillData, _owner);

            try
            {
                //이펙트 대기시간
                await UniTask.WaitForSeconds(1, cancellationToken:_skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
            }
            
            generatable.Release();
        }
    }
}