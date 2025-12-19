// using DG.Tweening;
// using MewVivor.Data;
// using MewVivor.Enum;
// using MewVivor.Managers;
// using UnityEngine;
//
// namespace MewVivor.InGame.Skill
// {
//     public class ArrowShotBehaviour : Projectile
//     {
//         public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
//         {
//             transform.position = spawnPosition;
//             transform.localScale = Vector3.zero;
//             transform.DOKill();
//             transform.DOScale(Vector3.one * skillData.ScaleMultiplier, 0.2f);
//             gameObject.SetActive(true);
//             _rigidbody.linearVelocity = direction * skillData.ProjSpeed;
//             transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
//             Manager.I.Audio.Play(Sound.Effect, "ArrowShot_Start");
//             Invoke(nameof(Release), 4);
//         }
//
//         public override void OnChangedSkillData(SkillData skillData)
//         {
//             transform.localScale = Vector3.one * skillData.ScaleMultiplier;
//         }
//     }
// }