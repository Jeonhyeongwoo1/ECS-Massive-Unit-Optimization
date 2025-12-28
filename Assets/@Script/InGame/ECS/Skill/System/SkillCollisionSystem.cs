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
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<SkillTag>();
        state.RequireForUpdate<MonsterComponent>();
        state.RequireForUpdate<PlayerInfoComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var skillLookup = SystemAPI.GetComponentLookup<SkillTag>(true); // ReadOnly
        var monsterLookup = SystemAPI.GetComponentLookup<MonsterComponent>(); // Read-Write
        var skillInfoLookup = SystemAPI.GetComponentLookup<SkillInfoComponent>(); // ReadOnly
        var playerInfoComponent = SystemAPI.GetSingleton<PlayerInfoComponent>();
        var skillHitEntityBufferLookup = SystemAPI.GetBufferLookup<SkillHitEntityBufferData>();
        uint distinctSeed = (uint)SystemAPI.Time.ElapsedTime + 1;

        skillLookup.Update(ref state);
        monsterLookup.Update(ref state);
        skillInfoLookup.Update(ref state);
        skillHitEntityBufferLookup.Update(ref state);

        // 3. Job 예약
        state.Dependency = new SkillTriggerJob
        {
            SkillLookup = skillLookup,
            MonsterLookup = monsterLookup,
            SkillInfoComponentLookup = skillInfoLookup,
            PlayerInfoComponent = playerInfoComponent,
            ECB = ecb.AsParallelWriter(),
            BaseSeed = distinctSeed,
            SkillHitEntityBufferLookup = skillHitEntityBufferLookup,
            DeltaTime = (float)SystemAPI.Time.ElapsedTime
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public struct SkillTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<SkillTag> SkillLookup;
        [ReadOnly] public ComponentLookup<SkillInfoComponent> SkillInfoComponentLookup;
        [ReadOnly] public ComponentLookup<MonsterComponent> MonsterLookup;
        [ReadOnly] public BufferLookup<SkillHitEntityBufferData> SkillHitEntityBufferLookup;
        [ReadOnly] public PlayerInfoComponent PlayerInfoComponent;
        
        public EntityCommandBuffer.ParallelWriter ECB;
        public uint BaseSeed;
        public float DeltaTime;

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
            SkillInfoComponent skillInfoComponent = SkillInfoComponentLookup[skill];
            if (SkillHitEntityBufferLookup.HasBuffer(skill))
            {
                var hitBuffer = SkillHitEntityBufferLookup[skill];
                for (int i = 0; i < hitBuffer.Length; i++)
                {
                    if (hitBuffer[i].Entity == monster)
                    {
                        return;
                    }
                }
            }

            uint seed = BaseSeed ^ (uint)monster.Index ^ (uint)skill.Index ^ (uint)(sortOffset * 7919);
            bool isCritical = false;
            float damage = 0;
            (damage, isCritical) = ECSExtensions.GetDamage(skillInfoComponent, PlayerInfoComponent, seed);
            var monsterTakeDamagedEventComponent = new MonsterTakeDamagedEventComponent
            {
                MonsterEntity = monster,
                SkillEntity = skill,
                Damage = damage,
                IsCritical = isCritical
            };

            var entity = ECB.CreateEntity(0);
            ECB.AddComponent(0, entity, monsterTakeDamagedEventComponent);
            ECB.AppendToBuffer(sortOffset, skill, new SkillHitEntityBufferData()
            {
                Entity = monster
            });
        }
    }
}