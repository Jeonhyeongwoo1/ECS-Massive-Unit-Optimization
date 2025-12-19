using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class SuicideBomberController : MonsterController
    {
        public override void Initialize(CreatureData creatureData, List<AttackSkillData> skillDataList)
        {
            _monsterType = MonsterType.SuicideBomber;
            base.Initialize(creatureData, skillDataList);
        }

        protected override void AttackProcess()
        {
            if (_player == null 
                || _player.IsDead 
                || _player.PlayerStateType == PlayerStateType.Invincibility)
            {
                return;
            }

            if (Utils.IsCollision(_collider, _player.Collider as CircleCollider2D))
            {
                _attackElapsedTime += Time.deltaTime;
                if (_attackElapsedTime > Const.MonsterAttackIntervalTime)
                {
                    _attackElapsedTime = 0;
                    // (기본 공격력 + ( StageLevel * 기본 공격력))
                    _player.TakeDamage(_creatureData.Atk, this);
                    Dead();
                }
            }
        }

        protected override async UniTask DeadAnimation()
        {
            _rigidbody.simulated = false;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(0, 0.2f).SetEase(Ease.InOutBounce));
            sequence.OnComplete(() =>
            {
                _rigidbody.simulated = true;
                transform.localScale = Vector3.one;
            });
            
            try
            {
                await UniTask.WaitForSeconds(0.2f);
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                Debug.LogError($"Error {e.Message}");
                return;
            }
        }
    }
}