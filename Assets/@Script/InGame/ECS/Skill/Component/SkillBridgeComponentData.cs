using MewVivor.Data;
using MewVivor.InGame.Skill;
using Unity.Entities;
using UnityEngine;


public interface IHitableObject
{
    public void OnHitMonsterEntity(Entity entity);
    public GameObject GameObject { get; }
}

public class SkillBridgeComponentData : IComponentData
{
    public IHitableObject hitableObject;
    public BaseSkillData BaseSkillData;
}

public struct SkillTag : IComponentData
{
    
}
