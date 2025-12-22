using System.Collections.Generic;
using MewVivor.Enum;
using Unity.Entities;

public class SkillSpawnPrefabData : IComponentData
{
    public Dictionary<AttackSkillType, Entity> PrefabDict = new();
}

public struct SkillSpawnTag : IComponentData
{
    
}