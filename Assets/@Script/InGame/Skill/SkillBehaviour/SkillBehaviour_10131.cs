using System.Collections;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10131 : Projectile
    {
        [SerializeField] private GameObject _ultimateObject;
        [SerializeField] private GameObject _normalObject;

        private Coroutine _skillLogicCor;
        private Camera _camera;
        
        public override void Generate(Vector3 spawnPosition, 
                                                Vector3 direction,
                                                AttackSkillData attackSkillData,
                                                CreatureController owner)
        {
            _camera = Camera.main;
            transform.localScale = Vector3.one * attackSkillData.Scale;
            transform.position = owner.Position;
            _ultimateObject.SetActive(attackSkillData.SkillCategoryType == SkillCategoryType.Ultimate);
            _normalObject.SetActive(attackSkillData.SkillCategoryType == SkillCategoryType.Normal);
            gameObject.SetActive(true);
            
            if (_skillLogicCor != null)
            {
                StopCoroutine(_skillLogicCor);
            }

            CreateBaseSkillEntity(attackSkillData);
            _skillLogicCor = StartCoroutine(SkillLogicCor(spawnPosition, direction, attackSkillData, owner));
        }

        //부메랑~
        private IEnumerator SkillLogicCor(Vector3 spawnPosition, Vector3 direction, AttackSkillData attackSkillData, CreatureController owner)
        {
            Vector3 targetPoint = spawnPosition + direction * 100;
            float secondSeqDuringTime = 0.7f;
            var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.ProjectileSpeed);
            float speed = Utils.CalculateStatValue(attackSkillData.ProjectileSpeed, modifer);

            var audio = Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10131, 0.5f, 0.15f);
            while (true)
            {
                direction = (targetPoint - transform.position).normalized;
                transform.Translate(direction * (speed * Time.deltaTime), Space.World);

                Vector3 viewPoint = _camera.WorldToViewportPoint(transform.position);
                if (viewPoint.x < 0.1f || viewPoint.x > 0.9f || viewPoint.y < 0.1f || viewPoint.y > 0.9f)
                {
                    break;
                }
                
                yield return new WaitForFixedUpdate();
            }
            
            yield return new WaitForSeconds(secondSeqDuringTime);
            
            while (true)
            {
                direction = (owner.transform.position - transform.position).normalized;
                transform.Translate(direction * (speed * Time.deltaTime), Space.World);
                
                float dist = (transform.position - owner.transform.position).sqrMagnitude;
                if (dist < 2)
                {
                    break;
                }
                
                yield return new WaitForFixedUpdate();
            }

            audio.GetAwaiter().GetResult()?.Stop();
            Release();
        }
    }
}