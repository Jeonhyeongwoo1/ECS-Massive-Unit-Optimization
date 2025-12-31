using MewVivor.Enum;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum BuffStateType
{
    Enter,
    Stay,
    Exit
}

[InternalBufferCapacity(8)]
public struct MonsterBuffElementData : IBufferElementData
{
    public DebuffType DebuffType;
    public float DebuffValuePercent;
    public float DebuffDuration;
    public float DebuffValue;
    public float ElapsedTime;
    public BuffStateType BuffStateType;
    public Entity SkillEntity;
    public bool RemoveOnTriggerExit;

    public void OnEnter(ref MonsterComponent monsterComponent)
    {
        switch (DebuffType)
        {
            case DebuffType.Stun:
                monsterComponent.UpdateCreatureStateType(CreatureStateType.Stun);
                DebuffDuration = DebuffValue;
                break;
            case DebuffType.AddDamage:
                break;
            case DebuffType.SlowSpeed:
                float value = monsterComponent.Speed * (1 - DebuffValue);
                monsterComponent.Speed = math.max(0, value);
                break;
            case DebuffType.DotDamage:
                break;
        }
        
        BuffStateType = BuffStateType.Stay;
    }

    public void OnStay(float deltaTime, ref MonsterComponent monsterComponent)
    {
        if (ElapsedTime > DebuffDuration)
        {
            BuffStateType = BuffStateType.Exit;
            // OnExit(ref monsterComponent);
        }
        
        ElapsedTime += deltaTime;
    }

    public void OnExit(ref MonsterComponent monsterComponent)
    {
        switch (DebuffType)
        {
            case DebuffType.Stun:
                monsterComponent.UpdateCreatureStateType(CreatureStateType.Move);
                break;
            case DebuffType.AddDamage:
                break;
            case DebuffType.SlowSpeed:
                monsterComponent.Speed = monsterComponent.oringSpeed;
                break;
            case DebuffType.DotDamage:
                break;
        }
    }
}
