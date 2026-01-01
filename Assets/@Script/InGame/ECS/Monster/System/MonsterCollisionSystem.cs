using MewVivor.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct MonsterCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<MonsterTag>();
        state.RequireForUpdate<PlayerInfoComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerInfoComponent = SystemAPI.GetSingleton<PlayerInfoComponent>();
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        state.Dependency = new CollisionJob()
        {
            PlayerInfoComponent = playerInfoComponent,
            ecb = ecb.AsParallelWriter(),
            DeltaTime = SystemAPI.Time.DeltaTime,
            AttackInterval = Const.MonsterAttackIntervalTime
        }.ScheduleParallel(state.Dependency);
        
    }

    [BurstCompile]
    public partial struct CollisionJob : IJobEntity
    {
        [ReadOnly] public PlayerInfoComponent PlayerInfoComponent;

        public EntityCommandBuffer.ParallelWriter ecb;
        public float DeltaTime;
        public float AttackInterval;

        void Execute([EntityIndexInQuery] int sortKey, 
            ref MonsterComponent monsterComponent,
            ref LocalTransform monsterTransform, 
            Entity entity)
        {
            float3 monsterPosition = monsterTransform.Position;
            float dist = math.distance(PlayerInfoComponent.Position, monsterPosition);
            float monsterRadius = monsterComponent.Radius;
            float playerRadius = PlayerInfoComponent.Radius;
            float resultRadius = monsterRadius + playerRadius - 0.5f;//offest;
            if (dist < resultRadius)
            {
                monsterComponent.AttackElapsedTime += DeltaTime;
                if (monsterComponent.AttackElapsedTime > AttackInterval)
                {
                    var newEntity = ecb.CreateEntity(sortKey);
                    ecb.AddComponent(sortKey, newEntity, new MonsterAttackEventComponent()
                    {
                        MonsterEntity = entity
                    });
                    
                    monsterComponent.AttackElapsedTime = 0;
                }
            }
            else
            {
                monsterComponent.AttackElapsedTime = 0;
            }
        }
    }
}
