using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Stat;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class Skill_10081 : RepeatAttackSkill
    {
        private IGeneratable _generatable;

        public override void Initialize(CreatureController owner, AttackSkillData attackSkillData)
        {
            base.Initialize(owner, attackSkillData);
            GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel, false);
            var generatable = prefab.GetComponent<IGeneratable>();
            generatable.OnHit = OnHit;
            _generatable = generatable;
            _generatable.ProjectileMono.gameObject.SetActive(false);
        }

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        protected override void Release()
        {
            base.Release();
            _generatable?.Release();
        }

        protected override async UniTask UseSkill()
        {
            StatModifer statModifer = _owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillDuration);
            float skillDuration = Utils.CalculateStatValue(AttackSkillData.SkillDuration, statModifer);
            _generatable.Generate(_owner.transform, Vector3.zero, AttackSkillData, _owner, CurrentLevel);
            var audio = await Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10081, 0.5f, 0.25f);
            try
            {
                await UniTask.WaitForSeconds(10, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                Debug.LogError($"error {nameof(UseSkill)} / {e.Message}");
                Release();
                return;
            }

            if (audio != null)
            {
                audio.Stop();
            }
            
            Release();
        }
    }
}