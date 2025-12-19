using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class GoldDropItemController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);

            _dropableItemType = DropableItemType.Gold;

            float scale = 1;
            if (dropItemData.DataId == Const.ID_GOLD_10)
            {
                scale = 1.2f;
            }
            else if (dropItemData.DataId == Const.ID_GOLD_50)
            {
                scale = 1.3f;
            }
            else if (dropItemData.DataId == Const.ID_GOLD_100)
            {
                scale = 1.4f;
            }
            else if (dropItemData.DataId == Const.ID_GOLD_300)
            {
                scale = 1.5f;
            }

            transform.localScale = Vector3.one * scale;
        }
    }
}