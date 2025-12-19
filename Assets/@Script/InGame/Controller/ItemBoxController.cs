using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class ItemBoxController : DropItemController , IHitable
    {
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);

            _dropableItemType = DropableItemType.ItemBox;
        }

        public void TakeDamage(float damage, CreatureController attacker)
        {
            if (IsRelease)
            {
                return;
            }
            
            DropItemData item = Utils.GetDropItemControllerByItemBox();
            Manager.I.Game.SpawnDropItem(item, transform.position);
            Manager.I.Game.CurrentMapController.Grid.RemoveItem(this);
            Release();
        }
    }
}