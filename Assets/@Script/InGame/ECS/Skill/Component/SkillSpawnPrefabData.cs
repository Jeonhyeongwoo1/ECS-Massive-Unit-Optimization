using System.Collections.Generic;
using MewVivor.Enum;
using Unity.Entities;

public struct SkillEntityData
{
    public AttackSkillType AttackSkillType;
    public Entity Entity;
}

public class SkillSpawnPrefabData : IComponentData
{
    public List<SkillEntityData> SkillPrefabList;
}

public struct SkillSpawnTag : IComponentData
{
}