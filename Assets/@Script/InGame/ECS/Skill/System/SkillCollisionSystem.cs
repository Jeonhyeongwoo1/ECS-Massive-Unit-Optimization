using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Random = Unity.Mathematics.Random; // Random 사용을 위해

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
        var monsterLookup = SystemAPI.GetComponentLookup<MonsterComponent>(false); // Read-Write
        var skillInfoLookup = SystemAPI.GetComponentLookup<SkillInfoComponent>(true); // ReadOnly
        var playerInfoComponent = SystemAPI.GetSingleton<PlayerInfoComponent>();

        // 2. 현재 시간을 시드 생성에 활용하기 위해 가져옴
        uint distinctSeed = (uint)SystemAPI.Time.ElapsedTime + 1;

        skillLookup.Update(ref state);
        monsterLookup.Update(ref state);
        skillInfoLookup.Update(ref state);
        
        // 3. Job 예약
        state.Dependency = new SkillCollisionJob
        {
            SkillLookup = skillLookup,
            MonsterLookup = monsterLookup,
            SkillInfoComponent = skillInfoLookup,
            PlayerInfoComponent = playerInfoComponent,
            ECB = ecb.AsParallelWriter(),
            BaseSeed = distinctSeed // 시드 전달
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public struct SkillCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<SkillTag> SkillLookup;
        [ReadOnly] public ComponentLookup<SkillInfoComponent> SkillInfoComponent;
        
        public ComponentLookup<MonsterComponent> MonsterLookup; 
        
        public EntityCommandBuffer.ParallelWriter ECB; 
        [ReadOnly] public PlayerInfoComponent PlayerInfoComponent;
        
        public uint BaseSeed; // 외부에서 받아온 기본 시드

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            // 충돌 순서는 보장되지 않으므로 체크 로직 유지
            bool isAB = SkillLookup.HasComponent(entityA) && MonsterLookup.HasComponent(entityB);
            bool isBA = SkillLookup.HasComponent(entityB) && MonsterLookup.HasComponent(entityA);

            if (isAB) ProcessCollision(entityA, entityB, 0); // index 0 (쓰레드 구분용)
            else if (isBA) ProcessCollision(entityB, entityA, 1); // index 1
        }

        private void ProcessCollision(Entity skill, Entity monster, int sortOffset)
        {
            // 데이터 읽기
            MonsterComponent monsterComponent = MonsterLookup[monster];
            SkillInfoComponent skillInfoComponent = SkillInfoComponent[skill];

            uint seed = BaseSeed ^ (uint)monster.Index ^ (uint)skill.Index ^ (uint)(sortOffset * 7919);
            Random random = Random.CreateFromIndex(seed);

            float randomValue = random.NextFloat(0f, 1f); // 0.0 ~ 1.0
            
            bool isCritical = false;
            float damage = GetDamage(skillInfoComponent, PlayerInfoComponent);

            if (randomValue < PlayerInfoComponent.CriticalPercent)
            {
                isCritical = true;
                damage *= PlayerInfoComponent.CriticalDamagePercent;
            }

            var monsterTakeDamagedEventComponent = new MonsterTakeDamagedEventComponent
                {
                    MonsterEntity = monster,
                    Damage = damage,
                    IsCritical = isCritical
                };

            
            
            var entity = ECB.CreateEntity(0);
            ECB.AddComponent(0, entity, monsterTakeDamagedEventComponent);
        }
        
        private float GetDamage(SkillInfoComponent skillInfo, PlayerInfoComponent playerInfo)
        {
            float damage = playerInfo.Atk;
            if (skillInfo.DamagePercent > 0) damage *= skillInfo.DamagePercent;
            return damage;
        }
    }
}