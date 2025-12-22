using MewVivor.Data;
using Unity.Entities;
using UnityEngine;

public class SkillComponent : IComponentData
{
    public GameObject GameObjectReference;
    public BaseSkillData BaseSkillData;
}

public struct SkillTag : IComponentData
{
    
}
