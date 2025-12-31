using System.Collections.Generic;
using MewVivor.Enum;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public struct MonsterSpawnRequestComponent : IComponentData
{
    public int Count;
    public float3 PlayerPosition;
    public float Scale;
    public float Speed;
    public float Radius;
    public float Atk;
    public float MaxHP;
    public MonsterType MonsterType;
    public int SpawnedWaveIndex;
}

public struct MonsterSpawnTag : IComponentData
{
}

[BurstCompile]
public partial struct MonsterSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MonsterSpawnComponent>();
        state.RequireForUpdate<MonsterSpawnRequestComponent>();
        state.RequireForUpdate<MonsterSpawnTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //생성한 엔티티에다가 특정 컴포넌트를 추가하고 삭제는 직접적으로 할 수 없고 커맨드 버퍼를 사용해서 해야함.
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        // var enemySpawnComponent = SystemAPI.GetSingleton<MonsterSpawnRequestComponent>();
        var monsterComponent = SystemAPI.GetSingleton<MonsterSpawnComponent>();
        //랜덤 난수 생성기를 초기화
        var random = new Random(seed: 12345);
        foreach (var (monsterSpawnRequestComponent, entity) 
                 in SystemAPI.Query<RefRO<MonsterSpawnRequestComponent>>().WithEntityAccess())
        {
            MonsterSpawnRequestComponent component = monsterSpawnRequestComponent.ValueRO;
            int count = component.Count;
            //생성된 Enemy에 대한 초기화
            for (int i = 0; i < count; i++)
            {
                Entity enemyEntity = state.EntityManager.Instantiate(monsterComponent.MonsterEntity);
                float angle = random.NextFloat(5, math.PI * 2f); //0 ~ 360 각도
                float distance = random.NextFloat(10f, 20);
                float3 position = new float3(math.cos(angle) * distance, math.sin(angle) * distance, -1f) 
                                  + component.PlayerPosition;
                var enemyData = new MonsterComponent()
                {
                    Speed = component.Speed,
                    oringSpeed = component.Speed,
                    Radius = component.Radius,
                    Atk = component.Atk,
                    MaxHP = component.MaxHP,
                    CurrentHP = component.MaxHP,
                    MonsterType = component.MonsterType,
                    SpawnedWaveIndex = component.SpawnedWaveIndex,
                    StateType = CreatureStateType.Move
                };
                
                ecb.AddComponent(enemyEntity, enemyData);
                ecb.SetComponent(enemyEntity, LocalTransform.FromPositionRotationScale(
                    position, quaternion.identity, component.Scale));
            }
            
            ecb.DestroyEntity(entity);
        }
        
        //ecb 실행
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
