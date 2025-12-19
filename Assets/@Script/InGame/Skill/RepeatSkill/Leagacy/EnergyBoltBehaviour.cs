// using System;
// using MewVivor.Data;
// using MewVivor.Enum;
// using MewVivor.InGame.Skill;
// using MewVivor.Managers;
// using UnityEngine;
//
//
// namespace MewVivor.InGame.Entity
// {
//     public class EnergyBoltBehaviour : Projectile
//     {
//         public int BounceCount { get; private set; }
//         
//         public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
//         {
//             transform.position = spawnPosition;
//             gameObject.SetActive(true);
//             UpdateVelocity(direction, skillData.ProjSpeed);
//             Manager.I.Audio.Play(Sound.Effect, "EnergyBolt_Start");
//             Invoke(nameof(Release), 3);
//         }
//
//         public void Bounce(Vector3 direction, float speed)
//         {
//             BounceCount++;
//             UpdateVelocity(direction, speed);
//         }
//
//         private void UpdateVelocity(Vector3 direction, float speed)
//         {
//             _rigidbody.linearVelocity = direction * speed;
//         }
//
//         private void OnDisable()
//         {
//             BounceCount = 0;
//         }
//     }
// }