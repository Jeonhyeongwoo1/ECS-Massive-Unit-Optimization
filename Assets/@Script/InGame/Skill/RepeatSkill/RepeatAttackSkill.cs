using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Stat;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public abstract class RepeatAttackSkill : BaseAttackSkill
    {
        protected CancellationTokenSource _skillLogicCts;
        protected List<IGeneratable> _projectileList = new List<IGeneratable>();
        
        public override async UniTask StartSkillLogicProcessAsync()
        {
            _skillLogicCts = new CancellationTokenSource();
            while (_skillLogicCts != null && !_skillLogicCts.IsCancellationRequested)
            {
                try
                {
                    await UseSkill();
                    float coolTime = CalculateCoolTime();
                    await UniTask.WaitForSeconds(coolTime, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    if (_skillLogicCts != null)
                    {
                        Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
                    }
                    break;
                }
            }
        }

        protected virtual float CalculateCoolTime()
        {
            float coolTime = AttackSkillData.CoolTime;
            if (_owner.SkillBook == null)
            {
                return coolTime;
            }
            
            var skillModifer = _owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.CoolTime);
            if (skillModifer != null)
            {
                bool isLinked = _owner.SkillBook.IsContainLinkedSkillId(PassiveSkillType.CoolTime, _skillData.DataId);
                if (isLinked)
                {
                    coolTime = Utils.CalculateStatValue(coolTime, skillModifer);
                }
            }
            
            CreatureStat skillCoolTime = (_owner as PlayerController).SkillCoolTime;
            if (skillCoolTime != null)
            {
                coolTime = Math.Max(0, (1 - skillCoolTime.Value) * coolTime);
            }
            
            return coolTime;
        }

        protected virtual void Release()
        {
            try
            {
                if (_projectileList.Count > 0)
                {
                    foreach (IGeneratable generatable in _projectileList)
                    {
                        generatable.Release();
                    }

                    _projectileList.Clear();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}