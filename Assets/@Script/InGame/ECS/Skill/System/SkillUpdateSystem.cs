using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct SkillUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerInfoComponent>();
        state.RequireForUpdate<SkillCollisionMapData>();
        state.RequireForUpdate<SkillTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(world: state.WorldUnmanaged);
        var skillCollisionMapData = SystemAPI.GetSingleton<SkillCollisionMapData>();
        BufferLookup<MonsterBuffElementData> monsterBuffLookup = SystemAPI.GetBufferLookup<MonsterBuffElementData>();
        NativeParallelMultiHashMap<Entity, Entity> collisionMapData = skillCollisionMapData.CollisionMap;
        var currentFrameCount = Time.frameCount;
        state.Dependency = new SkillLogicJob
        {
            // ReadOnly 데이터
            CollisionMapData = collisionMapData,
            CurrentFrameCount = currentFrameCount,
            DeltaTime = SystemAPI.Time.DeltaTime,
            ecb = ecb.AsParallelWriter(),
            MonsterBuffLookup = monsterBuffLookup
        }.ScheduleParallel(state.Dependency);
        
        state.Dependency.Complete();
    }
    
    [BurstCompile]
    public partial struct SkillLogicJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public float DeltaTime;
        public int CurrentFrameCount;
        [ReadOnly]
        public NativeParallelMultiHashMap<Entity, Entity> CollisionMapData;
        [NativeDisableParallelForRestriction] //쓰기작업 x
        public BufferLookup<MonsterBuffElementData> MonsterBuffLookup;

        public void Execute([EntityIndexInQuery] int sortIndex, ref SkillInfoComponent skillInfoComponent,
            ref DynamicBuffer<SkillHitEntityBufferData> hitBuffer, Entity skillEntity)
        {
            if (CollisionMapData.TryGetFirstValue(skillEntity, out var monsterEntity, out var it))
            {
                do
                {
                    //Buffer확인
                    bool found = false;
                    for (int i = 0; i < hitBuffer.Length; i++)
                    {
                        if (hitBuffer[i].Entity == monsterEntity)
                        {
                            ref var data = ref hitBuffer.ElementAt(i);
                            data.FrameCount = CurrentFrameCount;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var skill = skillInfoComponent;
                        hitBuffer.Add(new SkillHitEntityBufferData
                        {
                            Entity = monsterEntity,
                            FrameCount = CurrentFrameCount
                        });

                        //단발성 공격, 인터벌 공격 처음의 경우는 공격하기
                        if (!skill.IsIntervalAttack || skill.AttackElapsedTime == 0)
                        {
                            RequestMonsterTakeDamageEvent(skillInfoComponent, skillEntity, monsterEntity,
                                ecb, sortIndex);
                        }
                    }

                } while (CollisionMapData.TryGetNextValue(out monsterEntity, ref it));
            }

            for (int i = hitBuffer.Length - 1; i >= 0; i--)
            {
                SkillHitEntityBufferData buffer = hitBuffer[i];
                if (buffer.FrameCount != CurrentFrameCount)
                {
                    if (MonsterBuffLookup.HasBuffer(buffer.Entity))
                    {
                        var monsterBuffBuffer = MonsterBuffLookup[buffer.Entity];
                        for (int j = monsterBuffBuffer.Length - 1; j >= 0; j--)
                        {
                            ref var data = ref monsterBuffBuffer.ElementAt(j);
                            if (data.SkillEntity == skillEntity && data.RemoveOnTriggerExit)
                            {
                                data.BuffStateType = BuffStateType.Exit;
                            }
                        }
                    }
                    hitBuffer.RemoveAt(i);
                }
            }

            skillInfoComponent.AttackElapsedTime += DeltaTime;
            if (skillInfoComponent.IsIntervalAttack &&
                skillInfoComponent.IntervalAttackTime < skillInfoComponent.AttackElapsedTime)
            {
                skillInfoComponent.AttackElapsedTime = 0;
                for (int i = 0; i < hitBuffer.Length; i++)
                {
                    RequestMonsterTakeDamageEvent(skillInfoComponent, skillEntity, hitBuffer[i].Entity,
                        ecb, sortIndex);
                }
            }
        }

        private void RequestMonsterTakeDamageEvent(SkillInfoComponent skillInfoComponent, Entity skillEntity,
            Entity monsterEntity, EntityCommandBuffer.ParallelWriter ecb, int sortKey)
        {
            //requestData
            var monsterTakeDamagedEventComponent = new MonsterTakeDamagedEventComponent
            {
                MonsterEntity = monsterEntity,
                SkillEntity = skillEntity
            };

            var newEntity = ecb.CreateEntity(sortKey);
            ecb.AddComponent(sortKey, newEntity, monsterTakeDamagedEventComponent);
            skillInfoComponent.CurrentAttackCount++;
        }
    }
}