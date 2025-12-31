using System.Collections.Generic;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Entity;
using MewVivor.InGame.Enum;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill.SKillBehaviour
{
    public class SkillBehaviour_10081 : Projectile
    {
        [SerializeField] private List<TriggerNotifier> _projectileTriggerNotiferList;
        [SerializeField] private List<SpriteRenderer> _spriteRendererList;
        private List<TriggerNotifier> _activateProjectileTriggerNotiferList = new(6);
        private Tween _tween;

        private List<Unity.Entities.Entity> _entities = new List<Unity.Entities.Entity>(6);
        
        public override void Generate(Transform targetTransform, Vector3 direction, AttackSkillData attackSkillData,
            CreatureController owner, int currentLevel)
        {
            transform.SetParent(owner.transform);
            transform.localPosition = Vector3.zero;
            _activateProjectileTriggerNotiferList.Clear();
            foreach (var trigger in _projectileTriggerNotiferList)
            {
                trigger.gameObject.SetActive(false);
            }
            
            int count = attackSkillData.NumOfProjectile;
            for (int i = 0; i < count; i++)
            {
                float radius = attackSkillData.AttackRange;
                float angle = (360f / count) * i * Mathf.Deg2Rad; // 각도 분배
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                Vector3 position = new Vector3(x, y) + owner.Position;
                TriggerNotifier target = _projectileTriggerNotiferList[i];
                // CreateBaseSkillEntity(attackSkillData)
                // target.Initialize(Tag.Monster,
                //     (v) => OnHit.Invoke(v, this),
                //     null);

                var modifer = owner.SkillBook.GetPassiveSkillStatModifer(PassiveSkillType.SkillRange);
                float skillRange = Utils.CalculateStatValue(attackSkillData.Scale, modifer);
                target.transform.localScale = Vector3.one * skillRange;
                target.transform.position = position;
                target.gameObject.SetActive(true);
                _activateProjectileTriggerNotiferList.Add(target);

                Sprite sprite = Manager.I.Resource.Load<Sprite>(attackSkillData.SkillSprite);
                _spriteRendererList[i].sprite = sprite;
                target.Initialize(() =>
                {
                    var skill = CreateBaseSkillEntity(target, attackSkillData);
                    // _entities.Add(skill);
                });
            }

            gameObject.SetActive(true);
            if (_tween != null)
            {
                _tween.Kill();
            }

            transform.rotation = Quaternion.identity; // 회전 초기화
            _tween = transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        public override void Release()
        {
            if (_tween != null)
            {
                _tween.Kill();
            }
            
            _activateProjectileTriggerNotiferList.ForEach(v => v.Release());
        }
    }
}