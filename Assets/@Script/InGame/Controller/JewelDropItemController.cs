using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class JewelDropItemController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);

            _dropableItemType = DropableItemType.Jewel;

            float scale = 1;
            if (dropItemData.DataId == Const.ID_JEWEL_5)
            {
                scale = 1.3f;
            }
            else if (dropItemData.DataId == Const.ID_JEWEL_10)
            {
                scale = 1.5f;
            }

            transform.localScale = Vector3.one * scale;
        }
    }
}