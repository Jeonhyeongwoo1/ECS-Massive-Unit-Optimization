using System.Collections;
using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class PotionItemController : DropItemController
    {
        public override void Spawn(Vector3 spawnPosition, DropItemData dropItemData)
        {
            base.Spawn(spawnPosition, dropItemData);

            _dropableItemType = DropableItemType.Potion;
        }
        
        public override void Release()
        {
            base.Release();

            string resourceName = "Heal";
            var prefab = Manager.I.Resource.Instantiate(resourceName);
            prefab.transform.position = transform.position;
            prefab.gameObject.SetActive(true);
            Utils.DelayCallAsync(2, () => Manager.I.Pool.ReleaseObject(resourceName, prefab)).Forget();
        }
    }
}