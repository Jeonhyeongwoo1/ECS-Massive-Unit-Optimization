using DG.Tweening;
using MewVivor;
using MewVivor.Enum;
using MewVivor.Key;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MonsterTakeDamagedSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        foreach (var (damageEvent, eventEntity) 
                 in SystemAPI.Query<RefRO<MonsterTakeDamagedEventComponent>>()
                     .WithEntityAccess())
        {
            if (eventEntity == Entity.Null || damageEvent.ValueRO.MonsterEntity == Entity.Null)
            {
                continue;
            }
            
            // Debug.Log("SSSSS");
            Entity targetMonster = damageEvent.ValueRO.MonsterEntity;
            float damage = damageEvent.ValueRO.Damage;

            if (SystemAPI.HasComponent<MonsterComponent>(targetMonster))
            {
                var monsterRef = SystemAPI.GetComponentRW<MonsterComponent>(targetMonster);
                monsterRef.ValueRW.CurrentHP -= 10;
                var monsterTransform = SystemAPI.GetComponentRO<LocalTransform>(targetMonster);
                
                if (monsterRef.ValueRW.CurrentHP <= 0)
                {
                    ecb.DestroyEntity(targetMonster);
                }
                else
                {
                    var position = monsterTransform.ValueRO.Position;
                    var fontPosition = position + new float3(0, 1.5f, 0);
                    Manager.I.Object.ShowDamageFont(new float2(fontPosition.x, fontPosition.y),
                        damage,
                        0,
                        null,
                        damageEvent.ValueRO.IsCritical);

                    if (Manager.I.Object.IsInCameraView(position))
                    {
                        Manager.I.Audio.Play(Sound.SFX, SoundKey.MonsterHit, 0.5f, 0.15f);
                    }
                }
            }

            ecb.DestroyEntity(eventEntity);
        }
    }
}