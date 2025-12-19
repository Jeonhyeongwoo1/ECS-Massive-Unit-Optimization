using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class SkillUpItemController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);
            _dropableItemType = DropableItemType.SkillUp;
            Debug.Log("Spawn SKillUpItem");
        }
    }
}