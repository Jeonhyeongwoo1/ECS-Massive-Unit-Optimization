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
        if (!SystemAPI.ManagedAPI.TryGetSingleton<SkillSpawnPrefabData>(out var skillSpawnPrefabData))
        {
            return;
        }

        var skillEntityData =
            skillSpawnPrefabData.SkillPrefabList.Find(v => v.AttackSkillType == attackSkillData.AttackSkillType);
        Entity skillEntity = EntityManager.Instantiate(skillEntityData.Entity);
        EntityManager.AddComponentData(skillEntity, new SkillBridgeComponentData()
        {
            BaseSkillData = attackSkillData,
            GameObjectReference = referenceObject
        });

        EntityManager.AddComponentData(skillEntity, new SkillInfoComponent()
        {
            DamagePercent = attackSkillData.DamagePercent
        });
    }
}