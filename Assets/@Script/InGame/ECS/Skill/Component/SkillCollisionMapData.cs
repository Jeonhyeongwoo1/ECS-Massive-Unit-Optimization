using Unity.Entities;
using Unity.Collections;

public struct SkillCollisionMapData : IComponentData
{
    public NativeParallelMultiHashMap<Entity, Entity> CollisionMap;
}