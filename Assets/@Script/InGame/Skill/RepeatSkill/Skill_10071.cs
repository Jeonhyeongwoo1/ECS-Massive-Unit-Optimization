using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    /// <summary>
    ///  스킬 설명 :  캐릭터 주변에 사라지지 않는 자기장을 소환함.
    ///           - 자기장은 n초마다 주기적으로 데미지를 입힘.
    /// </summary>
    public class Skill_10071 : RepeatAttackSkill
    {
        private IGeneratable _generatable;
        private UniTaskCompletionSource _source;

        public override void Initialize(CreatureController owner, AttackSkillData attackSkillData)
        {
            base.Initialize(owner, attackSkillData);
            GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel, false);
            _generatable = prefab.GetComponent<IGeneratable>();
            _generatable.OnHit = OnHit;
            _generatable.OnExit = OnExit;
            _generatable.ProjectileMono.transform.SetParent(owner.transform);
            _generatable.ProjectileMono.gameObject.SetActive(false);
        }

        protected override async UniTask UseSkill()
        {
            var player = _owner as PlayerController;
            
            Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10071);
            _generatable.Generate(player.AttackPoint, _owner.GetDirection(), AttackSkillData, _owner, CurrentLevel);
            _source = new UniTaskCompletionSource();
            await _source.Task;
        }

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        protected override void Release()
        {
            base.Release();
            _source?.TrySetResult();
            _generatable?.Release();
        }

        public override void UpdateSkillData(BaseSkillData skillData)
        {
            base.UpdateSkillData(skillData);
            var player = _owner as PlayerController;
            _generatable.Generate(player.AttackPoint, _owner.GetDirection(), AttackSkillData, _owner, CurrentLevel);
        }
    }
}