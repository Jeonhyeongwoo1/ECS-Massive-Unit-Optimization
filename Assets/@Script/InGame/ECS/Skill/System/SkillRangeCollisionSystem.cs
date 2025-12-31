using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct SkillRangeCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerInfoComponent>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        CollisionWorld collisionWorld = 
                SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld.CollisionWorld;

        var collisionFilter = ECSHelper.CreateMonsterCollisionFilter();
        var monsterLookup = SystemAPI.GetComponentLookup<MonsterComponent>();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (skillRangeAttackComponentData, 
                     localTransform, 
                     skillInfoComponent, 
                     entity) 
                 in SystemAPI.Query<RefRW<SkillRangeAttackComponentData>, RefRO<LocalTransform>, RefRO<SkillInfoComponent>>().WithEntityAccess())
        {
            var hits = new NativeList<DistanceHit>(Allocator.Temp);
            float range = skillRangeAttackComponentData.ValueRO.Range;
            float3 position = localTransform.ValueRO.Position;
            if (collisionWorld.OverlapSphere(position, range, ref hits, collisionFilter))
            {
                foreach (DistanceHit hit in hits)
                {
                    if (!SystemAPI.Exists(hit.Entity) || !monsterLookup.HasComponent(hit.Entity))
                    {
                        continue;
                    }

                    var monsterTakeDamagedEventComponent = new MonsterTakeDamagedEventComponent
                    {
                        MonsterEntity = hit.Entity,
                        SkillEntity = entity,
                    };

                    var newEntity = ecb.CreateEntity();
                    ecb.AddComponent(newEntity, monsterTakeDamagedEventComponent);
                }
            }
            
            ecb.DestroyEntity(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}