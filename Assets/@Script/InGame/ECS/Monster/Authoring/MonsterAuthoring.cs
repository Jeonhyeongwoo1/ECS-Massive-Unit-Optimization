using Unity.Entities;
using UnityEngine;

public class MonsterAuthoring : MonoBehaviour
{

    [SerializeField] private float speed;
  
    private class MonsterAuthoringBaker : Baker<MonsterAuthoring>
    {
        public override void Bake(MonsterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new MonsterComponent()
            {
                Speed =  authoring.speed
            });

            AddComponent<MonsterTag>(entity);
        }
    }
}