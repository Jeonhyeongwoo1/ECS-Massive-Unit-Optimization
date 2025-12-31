using MewVivor.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct MonsterCollisionSystem : ISystem
{
    public PlayerInfoComponent playerInfoComponent;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<MonsterTag>();
        state.RequireForUpdate<PlayerInfoComponent>();
        playerInfoComponent = SystemAPI.GetSingleton<PlayerInfoComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPosition = playerInfoComponent.Position;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (monsterComponent,
                     monsterTransform,
                     entity)
                 in SystemAPI.Query<RefRW<MonsterComponent>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            if (entity == Entity.Null)
            {
                continue;
            }
            
            float3 monsterPosition = monsterTransform.ValueRO.Position;
            float dist = math.distance(playerPosition, monsterPosition);
            float monsterRadius = monsterComponent.ValueRO.Radius;
            float playerRadius = playerInfoComponent.Radius;
            float resultRadius = monsterRadius + playerRadius;
            if (dist < resultRadius)
            {
                monsterComponent.ValueRW.AttackElapsedTime += SystemAPI.Time.DeltaTime;
                if (monsterComponent.ValueRW.AttackElapsedTime > Const.MonsterAttackIntervalTime)
                {
                    var newEntity = ecb.CreateEntity();
                    ecb.AddComponent(newEntity, new MonsterAttackEventComponent()
                    {
                        MonsterEntity = entity
                    });
                    
                    monsterComponent.ValueRW.AttackElapsedTime = 0;
                }
            }
            else
            {
                monsterComponent.ValueRW.AttackElapsedTime = 0;
            }
        }
    }
}
