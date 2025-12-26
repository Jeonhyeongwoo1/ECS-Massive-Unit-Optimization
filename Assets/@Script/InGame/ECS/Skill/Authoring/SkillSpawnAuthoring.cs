using System;
using System.Collections.Generic;
using MewVivor.Enum;
using Unity.Entities;
using UnityEngine;

public class SkillSpawnAuthoring : MonoBehaviour
{
    [Serializable]
    public struct SkillPrefabData
    {
        public GameObject Prefab;
        public AttackSkillType SkillType;
    }

    public List<SkillPrefabData> SkillPrefabList;
    
    private class SkillSpawnAuthoringBaker : Baker<SkillSpawnAuthoring>
    {
        public override void Bake(SkillSpawnAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new SkillSpawnPrefabData
            {
                SkillPrefabList = new List<SkillEntityData>()
            };

            foreach (var item in authoring.SkillPrefabList) // 리스트 이름 가정
            {
                var prefabEntity = GetEntity(item.Prefab, TransformUsageFlags.Dynamic);
                SkillEntityData skillEntityData = new SkillEntityData
                {
                    AttackSkillType = item.SkillType,
                    Entity = prefabEntity
                };
                
                data.SkillPrefabList.Add(skillEntityData);
            }
            
            AddComponentObject(entity, data);
            AddComponent<SkillSpawnTag>(entity);
        }
    }
}