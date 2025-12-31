using System;
using MewVivor.Enum;
using Unity.Burst;
using Unity.Entities;

public partial struct MonsterBuffSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MonsterTag>();
        state.RequireForUpdate<MonsterBuffElementData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (monsterComponent, monsterBuffElementData, entity)
                 in SystemAPI.Query<RefRW<MonsterComponent>, DynamicBuffer<MonsterBuffElementData>>()
                     .WithEntityAccess())
        {
            int length = monsterBuffElementData.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                ref var buffElementData = ref monsterBuffElementData.ElementAt(i);
                if (!SystemAPI.Exists(buffElementData.SkillEntity) && buffElementData.RemoveOnTriggerExit)
                {
                    buffElementData.BuffStateType = BuffStateType.Exit;
                }

                switch (buffElementData.BuffStateType)
                {
                    case BuffStateType.Enter:
                        buffElementData.OnEnter(ref monsterComponent.ValueRW);
                        break;
                    case BuffStateType.Stay:
                        buffElementData.OnStay(deltaTime, ref monsterComponent.ValueRW);
                        break;
                    case BuffStateType.Exit:
                        buffElementData.OnExit(ref monsterComponent.ValueRW);
                        monsterBuffElementData.RemoveAt(i);
                        break;
                }
            }
        }
    }
}