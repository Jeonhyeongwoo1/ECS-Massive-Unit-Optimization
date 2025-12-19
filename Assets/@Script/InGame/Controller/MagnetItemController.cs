using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class MagnetItemController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);
            _dropableItemType = DropableItemType.Magnet;
        }
    }
}