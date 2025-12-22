using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct MonsterMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MonsterTag>();
        state.RequireForUpdate<PlayerInfoComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<PlayerInfoComponent>(out var playerInfoComponent))
        {
            return;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 playerPosition = playerInfoComponent.Position;

        state.Dependency = new MonsterMoveJob()
        {
            DeltaTime = deltaTime,
            PlayerPosition = playerPosition,
        }.ScheduleParallel(state.Dependency);

        // foreach (var (monsterTransform, monsterComponent) in SystemAPI
        //              .Query<RefRW<LocalTransform>, RefRO<MonsterComponent>>())
        // {
        //     float3 position = monsterTransform.ValueRO.Position;
        //     float3 direction = (playerPosition - position);
        //
        //     if (math.length(direction) > 0.1f)
        //     {
        //         direction = math.normalize(direction);
        //         monsterTransform.ValueRW.Position += direction * deltaTime * monsterComponent.ValueRO.Speed;
        //
        //         float yRotation = direction.x < 0 ? math.PI : 0f;
        //         monsterTransform.ValueRW.Rotation = quaternion.RotateY(yRotation);
        //     }
        // }
    }
}

[BurstCompile]
public partial struct MonsterMoveJob : IJobEntity
{
    public float DeltaTime;
    public float3 PlayerPosition;
        
    void Execute(ref LocalTransform monsterTransform, ref MonsterComponent monsterComponent, Entity entity)
    {
        float3 position = monsterTransform.Position;
        float3 direction = (PlayerPosition - position);

        if (math.length(direction) > 0.1f)
        {
            direction = math.normalize(direction);
            monsterTransform.Position += direction * DeltaTime * monsterComponent.Speed;

            float yRotation = direction.x < 0 ? math.PI : 0f;
            monsterTransform.Rotation = quaternion.RotateY(yRotation);
        }
    }
}