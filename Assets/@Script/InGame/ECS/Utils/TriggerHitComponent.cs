using Unity.Entities;
using UnityEngine;

public class TriggerHitComponent : MonoBehaviour, IHitableObject
{
    public GameObject GameObject => gameObject;
    
    public void OnHitMonsterEntity(Entity entity)
    {
        
    }
}