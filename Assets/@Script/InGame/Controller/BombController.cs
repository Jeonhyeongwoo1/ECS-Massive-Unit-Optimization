using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class BombController : DropItemController
    {
        private int _dataId;
        
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);

            _dropableItemType = DropableItemType.Bomb;

            float scale = 1;
            if (dropItemData.DataId == Const.ID_SMALL_BOMB)
            {
                scale = 1.3f;
            }
            else if (dropItemData.DataId == Const.ID_NORMAL_BOMB)
            {
                scale = 1.5f;
            }

            _dataId = dropItemData.DataId;
            transform.localScale = Vector3.one * scale;
        }

        public override void Release()
        {
            base.Release();
            
            string resourceName = _dataId == Const.ID_NORMAL_BOMB
                ? Const.Normal_Bomb_Effect_PrefabName
                : Const.Small_Bomb_Effect_PrefabName;
            var prefab = Manager.I.Resource.Instantiate(resourceName);
            prefab.transform.position = transform.position;
            prefab.gameObject.SetActive(true);
            Utils.DelayCallAsync(2, () => Manager.I.Pool.ReleaseObject(resourceName, prefab)).Forget();
            
        }
    }
}