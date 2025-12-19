// using MewVivor.Data;
// using MewVivor.Enum;
// using MewVivor.Managers;
// using UnityEngine;
//
// namespace MewVivor.InGame.Skill
// {
//     public class EgoSwordBehaviour : Projectile
//     {
//         public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
//         {
//             transform.position = spawnPosition;
//             transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
//             gameObject.SetActive(true);
//             _rigidbody.linearVelocity = direction * skillData.ProjSpeed;
//             Manager.I.Audio.Play(Sound.Effect, "EgoSword_Start");
//             Invoke(nameof(Release), 3);
//         }
//     }
// }