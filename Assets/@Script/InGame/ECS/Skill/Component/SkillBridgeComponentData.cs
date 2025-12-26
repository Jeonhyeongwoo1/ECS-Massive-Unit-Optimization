using MewVivor.Data;
using Unity.Entities;
using UnityEngine;

public class SkillBridgeComponentData : IComponentData
{
    public GameObject GameObjectReference;
    public BaseSkillData BaseSkillData;
}

public struct SkillTag : IComponentData
{
    
}
