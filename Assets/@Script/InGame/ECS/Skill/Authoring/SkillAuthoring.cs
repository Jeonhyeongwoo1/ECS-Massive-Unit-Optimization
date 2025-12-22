using Unity.Entities;
using UnityEngine;

namespace Script.InGame.ECS.Skill.Authoring
{
    public class SkillAuthoring : MonoBehaviour
    {
        private class SkillAuthoringBaker : Baker<SkillAuthoring>
        {
            public override void Bake(SkillAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<SkillTag>(entity);
            }
        }
    }
}