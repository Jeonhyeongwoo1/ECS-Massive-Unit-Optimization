using Cysharp.Threading.Tasks;
using MewVivor;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Key;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public struct MonsterDeadData
{
    public Vector3 Position;
    public MonsterType MonsterType;
    public int SpawnedWaveIndex;
}

public partial class SkillHitSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
        if (!SystemAPI.TryGetSingleton(out PlayerInfoComponent playerInfoComponent))
        {
            return;
        }

        var skillInfoComponentLookup = SystemAPI.GetComponentLookup<SkillInfoComponent>();
        var monsterBuffLookup = SystemAPI.GetBufferLookup<MonsterBuffElementData>();

        foreach (var (damageEvent, eventEntity)
                 in SystemAPI.Query<RefRO<MonsterTakeDamagedEventComponent>>()
                     .WithEntityAccess())
        {
            if (eventEntity == Entity.Null
                || damageEvent.ValueRO.MonsterEntity == Entity.Null
                || !SystemAPI.Exists(damageEvent.ValueRO.MonsterEntity)
                || !SystemAPI.Exists(damageEvent.ValueRO.SkillEntity))
            {
                ecb.DestroyEntity(eventEntity);
                continue;
            }

            var skillEntity = damageEvent.ValueRO.SkillEntity;
            var monsterEntity = damageEvent.ValueRO.MonsterEntity;
            var skillInfoComponent = skillInfoComponentLookup[skillEntity];
            ProcessMonsterTakeDamage(skillEntity, monsterEntity, ecb, playerInfoComponent, skillInfoComponent,
                monsterBuffLookup);
            ecb.DestroyEntity(eventEntity);
        }
    }

    private void ProcessMonsterTakeDamage(Entity skillEntity, Entity monsterEntity, EntityCommandBuffer ecb,
        PlayerInfoComponent playerInfoComponent, SkillInfoComponent skillInfoComponent,
        BufferLookup<MonsterBuffElementData> monsterBuffElementDataLookup)
    {
        if (SystemAPI.HasComponent<MonsterComponent>(monsterEntity))
        {
            uint seed = (uint)monsterEntity.Index ^ (uint)skillEntity.Index ^ (uint)SystemAPI.Time.ElapsedTime;
            bool isCritical = false;
            float damage = 0;
            (damage, isCritical) = ECSHelper.GetDamage(skillInfoComponent, playerInfoComponent, monsterEntity,
                monsterBuffElementDataLookup, seed);
            // damage = 30;
            var monsterRef = SystemAPI.GetComponentRW<MonsterComponent>(monsterEntity);
            monsterRef.ValueRW.CurrentHP -= damage;
            var monsterTransform = SystemAPI.GetComponentRO<LocalTransform>(monsterEntity);
            var skillBridgeComponentData =
                SystemAPI.ManagedAPI.GetComponent<SkillBridgeComponentData>(skillEntity);

            if (monsterRef.ValueRW.CurrentHP <= 0)
            {
                ecb.DestroyEntity(monsterEntity);
                var monsterDeadData = new MonsterDeadData
                {
                    Position = monsterTransform.ValueRO.Position,
                    MonsterType = monsterRef.ValueRO.MonsterType,
                    SpawnedWaveIndex = monsterRef.ValueRO.SpawnedWaveIndex
                };

                Manager.I.Object.DeadMonster(monsterDeadData);
            }
            else
            {
                var position = monsterTransform.ValueRO.Position;
                var fontPosition = position + new float3(0, 1.5f, 0);
                // Manager.I.Object.ShowDamageFont(new float2(fontPosition.x, fontPosition.y),
                //     damage,
                //     0,
                //     null,
                //     isCritical);

                if (Manager.I.Object.IsInCameraView(position))
                {
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.MonsterHit, 0.5f, 0.15f).Forget();
                }

                if (monsterRef.ValueRO.MonsterType == MonsterType.Boss)
                {
                    float ratio = monsterRef.ValueRO.CurrentHP == 0
                        ? 0
                        : monsterRef.ValueRO.CurrentHP / monsterRef.ValueRO.MaxHP;
                    Manager.I.Event.Raise(GameEventType.TakeDamageEliteOrBossMonster, ratio);
                }
                
                ApplyBuff(skillBridgeComponentData, monsterEntity, ecb, skillEntity, monsterBuffElementDataLookup);
            }

            skillBridgeComponentData.hitableObject?.OnHitMonsterEntity(monsterEntity);
        }
    }

    private void ApplyBuff(SkillBridgeComponentData skillBridgeComponentData, Entity monsterEntity,
        EntityCommandBuffer ecb, Entity skillEntity, BufferLookup<MonsterBuffElementData> monsterBuffElementDataLookup)
    {
        //Buff, Debuff
        var attackSkillData = skillBridgeComponentData.BaseSkillData as AttackSkillData;
        if (attackSkillData == null)
        {
            return;
        }

        // if (AttackSkillData.KnockbackDistance != 0)
        // {
        //     var monster = hitable as MonsterController;
        //     monster.ExecuteKnockback(AttackSkillData.KnockbackDistance);
        // }

        if (monsterBuffElementDataLookup.HasBuffer(monsterEntity))
        {
            var monsterBuff = monsterBuffElementDataLookup[monsterEntity];
            foreach (MonsterBuffElementData monsterBuffElementData in monsterBuff)
            {
                if (monsterBuffElementData.SkillEntity == skillEntity)
                {
                    return;
                }
            }
        }
        
        if (attackSkillData.DebuffType1 != DebuffType.None)
        {
            var monsterBuffElementData = ecb.AddBuffer<MonsterBuffElementData>(monsterEntity);
            var monsterBuffComponent = new MonsterBuffElementData()
            {
                DebuffType = attackSkillData.DebuffType1,
                DebuffValuePercent = attackSkillData.DebuffValuePercent1,
                DebuffDuration = attackSkillData.DebuffDuration1,
                DebuffValue = attackSkillData.DebuffValue1,
                ElapsedTime = 0,
                SkillEntity = skillEntity,
                RemoveOnTriggerExit = attackSkillData.DebuffType1 == DebuffType.AddDamage
            };

            monsterBuffElementData.Add(monsterBuffComponent);
        }

        if (attackSkillData.DebuffType2 != DebuffType.None)
        {
            var monsterBuffElementData = ecb.AddBuffer<MonsterBuffElementData>(monsterEntity);
            var monsterBuffComponent = new MonsterBuffElementData()
            {
                DebuffType = attackSkillData.DebuffType2,
                DebuffValuePercent = attackSkillData.DebuffValuePercent2,
                DebuffDuration = attackSkillData.DebuffDuration2,
                DebuffValue = attackSkillData.DebuffValue2,
                ElapsedTime = 0,
                SkillEntity = skillEntity,
                RemoveOnTriggerExit = attackSkillData.DebuffType2 == DebuffType.AddDamage
            };

            monsterBuffElementData.Add(monsterBuffComponent);
        }
    }

    public void AttackMonsterAndBossEntityListInFanShape(Entity skillEntity,
        Vector3 position, Vector3 direction, float radius,
        float angle = 180)
    {
        var singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = singleton.CreateCommandBuffer(World.Unmanaged);
        NativeList<Entity> list = GetMonsterAndBossEntityListInFanShape(position, direction, radius, angle);
        if (list.Length > 0)
        {
            foreach (Entity monsterEntity in list)
            {
                var monsterTakeDamagedEventComponent = new MonsterTakeDamagedEventComponent
                {
                    MonsterEntity = monsterEntity,
                    SkillEntity = skillEntity,
                };

                Entity monsterTakeDamagedEntity = ecb.CreateEntity();
                ecb.AddComponent(monsterTakeDamagedEntity, monsterTakeDamagedEventComponent);
            }
        }

        list.Dispose();
    }

    public NativeList<Entity> GetMonsterAndBossEntityListInFanShape(Vector3 position, Vector3 direction, float radius,
        float angle = 180)
    {
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;
        var collisionFilter = ECSHelper.CreateMonsterCollisionFilter();
        var hits = new NativeList<DistanceHit>(Allocator.Temp);
        var entityList = new NativeList<Entity>(Allocator.Temp);
        if (collisionWorld.OverlapSphere(position, radius, ref hits, collisionFilter))
        {
            if (hits.Length > 0)
            {
                foreach (DistanceHit hit in hits)
                {
                    float3 monsterPosition = hit.Position;
                    Vector3 inVector = (monsterPosition.ToVector3() - position).normalized;
                    float dot = Vector3.Dot(inVector, direction);
                    dot = Mathf.Clamp(dot, -1f, 1f);
                    float degree = Mathf.Acos(dot) * Mathf.Rad2Deg;
                    if (degree <= angle / 2)
                    {
                        entityList.Add(hit.Entity);
                    }
                }
            }
        }

        hits.Dispose();
        return entityList;
    }
}