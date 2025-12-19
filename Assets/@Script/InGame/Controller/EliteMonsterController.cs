using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Managers;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class EliteMonsterController : MonsterController
    {
        public override void Initialize(CreatureData creatureData, List<AttackSkillData> skillDataList)
        {
            _monsterType = MonsterType.Elite;
            base.Initialize(creatureData, skillDataList);
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            base.TakeDamage(damage, attacker);

            // float ratio = HP == 0 ? 0 : HP / _creatureData.MaxHp;
            // Manager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
        }

        public override void Spawn(Vector3 spawnPosition, PlayerController player, int spawnedWaveIndex)
        {
            transform.position = spawnPosition;
            _player = player;
            gameObject.SetActive(true);
            _spawnedWaveIndex = spawnedWaveIndex;

            StartCoroutine(BossAndEliteSpawnAnimationCor(false));
        }

        public override void TakeBombSkill()
        {
            int damage = (int)(MaxHP.Value * 0.2f);
            TakeDamage(damage, null);
        }
    }
}