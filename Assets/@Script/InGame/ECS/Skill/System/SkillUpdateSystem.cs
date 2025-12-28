using Unity.Burst;
using Unity.Entities;

public partial struct SkillUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SkillTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (skillInfoComponent, hitBuffer, entity)
                 in SystemAPI.Query<RefRW<SkillInfoComponent>, DynamicBuffer<SkillHitEntityBufferData>>()
                     .WithEntityAccess())
        {
            if (skillInfoComponent.ValueRO.IntervalAttackTime > skillInfoComponent.ValueRO.AttackElapsedTime)
            {
                skillInfoComponent.ValueRW.AttackElapsedTime += SystemAPI.Time.DeltaTime;
            }
            else
            {
                if (skillInfoComponent.ValueRO.IsIntervalAttack)
                {
                    hitBuffer.Clear();
                    skillInfoComponent.ValueRW.AttackElapsedTime = 0;
                }
                else
                {
                    //단발성 공격
                    skillInfoComponent.ValueRW.CurrentAttackCount++;
                }
            }
        }
    }
}