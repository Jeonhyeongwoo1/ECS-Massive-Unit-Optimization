using Unity.Entities;


public struct MonsterComponent : IComponentData
{
    public float Speed;
    public float Radius;
    public float Atk;

    public float AttackElapsedTime;
}

public struct MonsterTag : IComponentData
{
    
}