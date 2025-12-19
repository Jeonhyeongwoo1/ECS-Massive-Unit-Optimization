using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MewVivor.Managers
{
    public class PoolManager
    {
        private Dictionary<string, IObjectPool<GameObject>> _poolDict = new ();
        
        public GameObject GetObject(string name, GameObject prefab = null)
        {
            if (_poolDict.TryGetValue(name, out var objectPool))
            {
                GameObject obj = objectPool.Get();
                if (obj == null || obj.IsDestroyed())
                {
                    objectPool.Release(obj);
                }
                else
                {
                    return obj;
                }
            }
            
            var pool = CreatePool(prefab);
            _poolDict.TryAdd(name, pool);

            return pool.Get();
        }

        public void ReleaseObject(string name, GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            if (_poolDict.TryGetValue(name, out IObjectPool<GameObject> objectPool))
            {
                try
                {
                    objectPool.Release(obj);
                    obj.SetActive(false);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Release warning {e.Message}");
                }
            }
        }

        public void ClearDict()
        {   
            _poolDict.Clear();
        }

        private IObjectPool<GameObject> CreatePool(GameObject prefab)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Object.Instantiate(prefab);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnDestroy: Object.Destroy
            );
        }
    }
}