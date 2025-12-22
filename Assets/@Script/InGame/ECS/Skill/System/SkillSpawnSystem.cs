using MewVivor.Data;
using MewVivor.Enum;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class SkillSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
            
    }

    public void CreateSkill(GameObject referenceObject, AttackSkillData attackSkillData)
    {
        var skillSpawnPrefabData = SystemAPI.ManagedAPI.GetSingleton<SkillSpawnPrefabData>();
        Entity skillEntityRef =skillSpawnPrefabData.PrefabDict[AttackSkillType.Skill_10001];
        Entity skillEntity = EntityManager.Instantiate(skillEntityRef);

        EntityManager.AddComponentData(skillEntity, new SkillComponent()
        {
            BaseSkillData = attackSkillData,
            GameObjectReference = referenceObject
        });
    }
}