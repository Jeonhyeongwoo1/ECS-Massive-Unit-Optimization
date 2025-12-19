using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10061 : Projectile
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SectorGizmoDrawer _sectorGizmoDrawer;
        
        public override void Generate(Transform targetTransform, Vector3 direction, AttackSkillData attackSkillData,
            CreatureController owner, int currentLevel)
        {
            gameObject.SetActive(true);
            if (attackSkillData.SkillCategoryType == SkillCategoryType.Ultimate)
            {
                _animator.Play("Skill_10061_ultimate");
                transform.localPosition = Vector3.zero;
            }
            else
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                float radius = attackSkillData.AttackRange;
                transform.rotation = rotation;
                transform.position = targetTransform.position + direction * -1 ;
            
                Vector3 rotatedDirection = rotation * Vector3.right;
                _animator.Play("Skill_10061");
                _sectorGizmoDrawer.SetData(attackSkillData.ConeAngle, radius, rotatedDirection);
            }
            
            transform.localScale = Vector3.one * attackSkillData.Scale;
        }
        
        public override void Release()
        {
            gameObject.SetActive(false);
        }
    }
}