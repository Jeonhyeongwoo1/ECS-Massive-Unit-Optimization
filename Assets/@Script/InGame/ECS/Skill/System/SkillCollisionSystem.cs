using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct SkillCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<SkillCollisionMapData>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<SkillTag>();
        state.RequireForUpdate<MonsterComponent>();
        state.RequireForUpdate<PlayerInfoComponent>();
        
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(entity, new SkillCollisionMapData
        {
            CollisionMap = new NativeParallelMultiHashMap<Entity, Entity>(10000, Allocator.Persistent)
        });
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (SystemAPI.TryGetSingleton(out SkillCollisionMapData skillCollisionMapData))
        {
            skillCollisionMapData.CollisionMap.Dispose();
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var skillLookup = SystemAPI.GetComponentLookup<SkillTag>(true); // ReadOnly
        var monsterLookup = SystemAPI.GetComponentLookup<MonsterComponent>(); // Read-Write
        var collisionMap = SystemAPI.GetSingleton<SkillCollisionMapData>().CollisionMap;

        collisionMap.Clear();
        // 3. Job 예약
        state.Dependency = new SkillTriggerJob
        {
            SkillLookup = skillLookup,
            MonsterLookup = monsterLookup,
            SkillHitEntityBufferMap = collisionMap.AsParallelWriter()
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        
        state.Dependency.Complete();
    }

    [BurstCompile]
    public struct SkillTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<SkillTag> SkillLookup;
        [ReadOnly] public ComponentLookup<MonsterComponent> MonsterLookup;
        
        public NativeParallelMultiHashMap<Entity, Entity>.ParallelWriter SkillHitEntityBufferMap;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            // 충돌 순서는 보장되지 않으므로 체크 로직 유지
            bool isAB = SkillLookup.HasComponent(entityA) && MonsterLookup.HasComponent(entityB);
            bool isBA = SkillLookup.HasComponent(entityB) && MonsterLookup.HasComponent(entityA);

            if (isAB)
            {
                ProcessCollision(entityA, entityB, 0); // index 0 (쓰레드 구분용)
            }
            else if (isBA)
            {
                ProcessCollision(entityB, entityA, 1); // index 1
            }
        }

        private void ProcessCollision(Entity skill, Entity monster, int sortOffset)
        {
            SkillHitEntityBufferMap.Add(skill, monster);
            
            // SkillInfoComponent skillInfoComponent = SkillInfoComponentLookup[skill];
            // if (SkillHitEntityBufferLookup.HasBuffer(skill))
            // {
            //     var hitBuffer = SkillHitEntityBufferLookup[skill];
            //     for (int i = 0; i < hitBuffer.Length; i++)
            //     {
            //         if (hitBuffer[i].Entity == monster)
            //         {
            //             return;
            //         }
            //     }
            // }

            // uint seed = BaseSeed ^ (uint)monster.Index ^ (uint)skill.Index ^ (uint)(sortOffset * 7919);
            // var monsterTakeDamagedEventComponent = new MonsterTakeDamagedEventComponent
            // {
            //     MonsterEntity = monster,
            //     SkillEntity = skill,
            // };
            //
            // var entity = ECB.CreateEntity(0);
            // ECB.AddComponent(0, entity, monsterTakeDamagedEventComponent);
            // ECB.AppendToBuffer(sortOffset, skill, new SkillHitEntityBufferData()
            // {
            //     Entity = monster
            // });
        }
    }
}