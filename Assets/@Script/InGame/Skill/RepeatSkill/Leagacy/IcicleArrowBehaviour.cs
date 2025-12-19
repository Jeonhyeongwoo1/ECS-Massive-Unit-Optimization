// using System;
// using System.Collections;
// using System.Collections.Generic;
// using MewVivor.Data;
// using MewVivor.Enum;
// using MewVivor.Managers;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace MewVivor.InGame.Skill
// {
//     public class IcicleArrowBehaviour : Projectile
//     {
//         public int PenerationCount
//         {
//             get => _penerationCount;
//             set
//             {
//                 _penerationCount = value;
//                 if (_penerationCount == 0)
//                 {
//                     Release();
//                 }
//             }
//         }
//
//         private int _penerationCount;
//         
//         public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
//         {
//             transform.position = spawnPosition;
//             transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
//             gameObject.SetActive(true);
//
//             _penerationCount = skillData.NumPenerations;
//             UpdateVelocity(direction, skillData.ProjSpeed);
//             Invoke(nameof(Release), 3);
//             Manager.I.Audio.Play(Sound.Effect, "IcicleArrow_Start");
//         }
//
//         private void UpdateVelocity(Vector3 direction, float speed)
//         {
//             _rigidbody.linearVelocity = direction * speed;
//         }
//     }
// }