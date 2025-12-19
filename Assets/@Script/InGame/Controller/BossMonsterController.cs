using System.Collections;
using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Entity;
using MewVivor.Key;
using MewVivor.Managers;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class BossMonsterController : MonsterController
    {
        public override void Initialize(CreatureData creatureData, List<AttackSkillData> skillDataList)
        {
            _creatureStateType = CreatureStateType.Skill;
            _monsterType = MonsterType.Boss;
            base.Initialize(creatureData, skillDataList);
        }

        public override void Spawn(Vector3 spawnPosition, PlayerController player, int spawnedWaveIndex)
        {
            transform.position = spawnPosition;
            _player = player;
            gameObject.SetActive(true);
            _spawnedWaveIndex = spawnedWaveIndex;

            StartCoroutine(BossAndEliteSpawnAnimationCor(true));
        }

        protected override void Dead()
        {
            base.Dead();
            Manager.I.Audio.Play(Sound.BGM, Manager.I.Game.GameType == GameType.MAIN ? SoundKey.BGM_Ingame : SoundKey.BGM_InfinityIngame, 1f, 0.35f);
        }

        public void UpdateState(CreatureStateType stateType) => _creatureStateType = stateType;

        private void UpdateAnimation(string animationName)
        {
            _animator.Play(animationName);
        }
        
        public override void UpdateStateAndAnimation(CreatureStateType stateType, string animationName)
        {
            UpdateState(stateType);
            UpdateAnimation(animationName);
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            base.TakeDamage(damage, attacker);
            float ratio = HP.Value == 0 ? 0 : HP.Value / MaxHP.Value;
            Manager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
        }

        public override void TakeBombSkill()
        {
        }
    }
}