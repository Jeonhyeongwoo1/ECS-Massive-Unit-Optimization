using System;
using System.Collections.Generic;
using MewVivor.Enum;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct SkillPrefabData
{
    public GameObject Prefab;
    public AttackSkillType SkillType;
}

public class SkillSpawnAuthoring : MonoBehaviour
{
    public List<SkillPrefabData> SkillPrefabList;
    public GameObject prefab;
    
    private class SkillSpawnAuthoringBaker : Baker<SkillSpawnAuthoring>
    {
        public override void Bake(SkillSpawnAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new SkillSpawnPrefabData();

            foreach (var item in authoring.SkillPrefabList) // 리스트 이름 가정
            {
                var prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
                data.PrefabDict.Add(item.SkillType, prefabEntity);
            }

            AddComponentObject(entity, data);
        }
    }
}