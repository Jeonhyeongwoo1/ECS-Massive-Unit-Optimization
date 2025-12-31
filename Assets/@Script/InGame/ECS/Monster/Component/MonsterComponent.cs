using System;
using MewVivor.Enum;
using Unity.Entities;

public struct MonsterComponent : IComponentData
{
    public float Speed;
    public float Radius;
    public float Atk;
    public float MaxHP;
    public float CurrentHP;
    public MonsterType MonsterType;
    public int SpawnedWaveIndex;

    //지속적으로 변화하는 데이터
    public float AttackElapsedTime;
    public CreatureStateType StateType;
    public float oringSpeed;
    
    public void UpdateCreatureStateType(CreatureStateType newState)
    {
        if (StateType == newState)
        {
            return;
        }

        StateType = newState;
    }
}

public struct MonsterTag : IComponentData
{
}