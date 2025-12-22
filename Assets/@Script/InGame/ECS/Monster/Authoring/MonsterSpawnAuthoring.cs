using Unity.Entities;
using UnityEngine;

namespace Script.InGame.ECS.Monster
{
    public class MonsterSpawnAuthoring : MonoBehaviour
    {
        public GameObject MonsterPrefab => monsterPrefab;

        [SerializeField] private GameObject monsterPrefab;
        [SerializeField] private int spawnCount = 3000;

        private class MonsterSpawnAuthoringBaker : Baker<MonsterSpawnAuthoring>
        {
            public override void Bake(MonsterSpawnAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new MonsterSpawnComponent()
                {
                    MonsterEntity = GetEntity(authoring.MonsterPrefab, TransformUsageFlags.Dynamic),
                });
                
                AddComponent<MonsterSpawnTag>(entity);
            }
        }
    }
}