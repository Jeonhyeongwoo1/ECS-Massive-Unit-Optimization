using System;
using DG.Tweening;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Stat;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Presenter;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MewVivor.Controller
{
    public class GameSceneController : BaseSceneController
    {
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var requestEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(requestEntity, new MonsterSpawnRequestComponent()
                {
                    Count = 1,
                    PlayerPosition = new float3(0,0,0),
                    Scale = 2.5f,
                    Speed = 2,
                    Radius = 2,
                    Atk = 10,
                    MaxHP = 13000,
                    MonsterType = MonsterType.Normal,
                    SpawnedWaveIndex = 1
                });
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var player = Manager.I.Object.Player;
                var modifer = new StatModifer(1000000, ModifyType.Flat, this);
                player.Atk.AddModifier(modifer);
                DOVirtual.DelayedCall(3, () => player.Atk.RemoveModifer(modifer));
            }
        }

        private void Initialize()
        {
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            var pausePopupPresenter = PresenterFactory.CreateOrGet<PausePopupPresenter>();
            pausePopupPresenter.Initialize(playerModel);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
        }
    }
}