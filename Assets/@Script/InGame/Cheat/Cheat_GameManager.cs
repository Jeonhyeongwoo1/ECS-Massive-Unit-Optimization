using System.Collections.Generic;
using System.Linq;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Controller;
using MewVivor.Model;
using UnityEngine;

namespace MewVivor
{
    public partial class GameManager
    {
        public void Cheat_SpawnMonster(int monsterId, int spawnCount, float scale)
        {
            MonsterController targetMonster = null;
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 playerSpawnPosition = Manager.I.Object.Player.transform.position
                                              + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10));

                CreatureData creatureData = Manager.I.Data.CreatureDict[monsterId];
                MonsterType monsterType = MonsterType.None;
                if (creatureData.PrefabLabel == "MonsterPrefab")
                {
                    monsterType = MonsterType.Normal;
                }
                else if (creatureData.PrefabLabel == "ElitePrefab")
                {
                    monsterType = MonsterType.Elite;
                }
                else if (creatureData.PrefabLabel == "BossPrefab")
                {
                    monsterType = MonsterType.Boss;
                }
                //
                // var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
                // int waveIndex = stageModel.CurrentWaveStep.Value;
                // MonsterController monster = Manager.I.Object.SpawnMonster(monsterId, playerSpawnPosition, monsterType, waveIndex);
                //
                // targetMonster = monster;
                // if (monster.MonsterType == MonsterType.Boss)
                // {
                //     CurrentStage.OnBossKill();
                //     Manager.I.Event.Raise(GameEventType.SpawnedBoss);
                // }

                Manager.I.Game.CurrentStage.RaiseSpawnMonster(1, new List<int>() { monsterId }, monsterType);
            }

            if (targetMonster != null)
            {
                List<MonsterController> list = Manager.I.Object.ActivateMonsterList
                    .Where(x => x.MonsterType == targetMonster.MonsterType).ToList();
                foreach (MonsterController monsterController in list)
                {
                    monsterController.Cheat_ResizeScale(scale);
                }
            }
        }
    }
}