using MewVivor.Enum;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public static class ECSHelper
{
    // 싱글톤이 있으면 값을 업데이트하고, 없으면 새로 만들어서 값을 넣는 함수
    public static void SetOrCreateSingleton<T>(this EntityManager entityManager, T data) 
        where T : unmanaged, IComponentData
    {
        var query = entityManager.CreateEntityQuery(typeof(T));

        if (query.CalculateEntityCount() == 0)
        {
            // 없으면 생성 + 값 설정
            var entity = entityManager.CreateEntity(typeof(T));
            entityManager.SetComponentData(entity, data);
        }
        else
        {
            // 있으면 값만 업데이트
            query.SetSingleton(data);
        }
    }

    public static Vector3 ToVector3(this float3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }
    
    public static (float, bool) GetDamage(SkillInfoComponent skillInfo, 
        PlayerInfoComponent playerInfo,
        Entity monsterEntity,
        BufferLookup<MonsterBuffElementData> monsterBuffElementDataLookup,
        uint seed)
    {
        float damage = playerInfo.Atk;
        if (skillInfo.DamagePercent > 0)
        {
            damage *= skillInfo.DamagePercent;
        }
        
        Random random = Random.CreateFromIndex(seed);
        float randomValue = random.NextFloat(0f, 1f); // 0.0 ~ 1.0
        bool isCritical = false;
        if (randomValue < playerInfo.CriticalPercent)
        {
            isCritical = true;
            damage *= playerInfo.CriticalDamagePercent;
        }

        if (monsterBuffElementDataLookup.HasBuffer(monsterEntity))
        {
            float percent = 0;
            var monsterBuffElementData = monsterBuffElementDataLookup[monsterEntity];
            foreach (MonsterBuffElementData buffElementData in monsterBuffElementData)
            {
                if (buffElementData.BuffStateType == BuffStateType.Exit)
                {
                    continue;
                }
                
                if (buffElementData.DebuffType == DebuffType.AddDamage)
                {
                    percent += buffElementData.DebuffValuePercent;
                }
            }

            damage *= (1 + percent);
        }

        return (damage, isCritical);
    }

    public static CollisionFilter CreateMonsterCollisionFilter()
    {
        var collisionFilter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = 1 << 6,
            GroupIndex = 0
        };

        return collisionFilter;
    }

    public static void RemoveBuff(BufferLookup<MonsterBuffElementData> monsterBuffLookup, Entity monsterEntity,
        Entity skillEntity)
    {
        if (monsterBuffLookup.HasBuffer(monsterEntity))
        {
            var monsterBuffBuffer = monsterBuffLookup[monsterEntity];
            for (int j = monsterBuffBuffer.Length - 1; j >= 0; j--)
            {
                ref var data = ref monsterBuffBuffer.ElementAt(j);
                if (data.SkillEntity == skillEntity && data.RemoveOnTriggerExit)
                {
                    data.BuffStateType = BuffStateType.Exit;
                }
            }
        }
    }
}