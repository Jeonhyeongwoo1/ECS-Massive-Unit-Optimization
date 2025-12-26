using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInfoComponent : IComponentData
{
    public float3 Position;
    public float Radius;
    public float CriticalPercent;
    public float CriticalDamagePercent;
    public float Atk;
}