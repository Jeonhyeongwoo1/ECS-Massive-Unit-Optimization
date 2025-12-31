using Unity.Entities;

[InternalBufferCapacity(40)]
public struct SkillHitEntityBufferData : IBufferElementData
{
    public long FrameCount;
    public Entity Entity;
}