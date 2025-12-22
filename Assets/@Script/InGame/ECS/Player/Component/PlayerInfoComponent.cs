using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInfoComponent : IComponentData
{
    public float3 Position;
    public float Radius;
}