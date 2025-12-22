using MewVivor;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class MonsterAttackSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (monsterAttackEventComponent, entity) in SystemAPI.Query<RefRO<MonsterAttackEventComponent>>().WithEntityAccess())
        {
            var monsterEntity = monsterAttackEventComponent.ValueRO.MonsterEntity;
            var monsterComponent = SystemAPI.GetComponentRO<MonsterComponent>(monsterEntity);

            var atk = monsterComponent.ValueRO.Atk;
            var player = Manager.I.Object.Player;

            if (!player.IsDead)
            {
                player.TakeDamage(atk);
            }
        
            ecb.DestroyEntity(entity);
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}