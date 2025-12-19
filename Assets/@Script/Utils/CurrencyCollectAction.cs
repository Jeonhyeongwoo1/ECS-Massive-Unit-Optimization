using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor
{
    public class CurrencyCollectAction : MonoBehaviour
    {
        public static CurrencyCollectAction I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find(nameof(CurrencyCollectAction)).GetComponent<CurrencyCollectAction>();
                }

                return _instance;
            }
        }
        
        private static CurrencyCollectAction _instance;
        private Camera Camera 
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
        }

        private Camera _camera;
        private List<UI_CurrencyObject> _onActivateList = new();
        private CancellationTokenSource _cts;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void ShowAnimation(CurrencyType currencyType, Vector3 endPoint, int spawnCurrencyCount = 10,
            Action callback = null)
        {
            if (_cts != null)
            {
                StopAnimation();
            }

            _cts = new CancellationTokenSource();
            Sprite currencySprite = GetCurrencySprite(currencyType);
            ShowAsync(currencySprite, spawnCurrencyCount, endPoint, callback).Forget();
        }

        public void StopAnimation()
        {
            Utils.SafeCancelCancellationTokenSource(ref _cts);

            foreach (UI_CurrencyObject uiCurrencyObject in _onActivateList)
            {
                uiCurrencyObject.ReleaseObject();
            }
            
            _onActivateList.Clear();
        }

        private Sprite GetCurrencySprite(CurrencyType currencyType)
        {
            Sprite sprite = null;
            switch (currencyType)
            {
                case CurrencyType.Gold:
                    sprite = Manager.I.Resource.Load<Sprite>(Const.GoldSpriteName);
                    break;
                case CurrencyType.Jewel:
                    sprite = Manager.I.Resource.Load<Sprite>(Const.JewelSpriteName);
                    break;
            }

            return sprite;
        }

        private async UniTaskVoid ShowAsync(Sprite currencySprite, int spawnCurrencyCount, Vector3 endPoint, Action callback)
        {
            int count = 0;
            for (int i = 0; i < spawnCurrencyCount; i++)
            {
                Vector3 screenPoint = Camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                Vector3 spawnPosition = screenPoint + new Vector3(Random.Range(-130f, 130f), Random.Range(-130f, 130f));
                var prefab = Manager.I.Resource.Instantiate(nameof(UI_CurrencyObject));
                prefab.transform.SetParent(transform);
                if (prefab.TryGetComponent(out UI_CurrencyObject currencyObject))
                {
                    currencyObject.Show(currencySprite, spawnPosition, endPoint, () =>
                    {
                        count++;
                        _onActivateList.Remove(currencyObject);
                        currencyObject.ReleaseObject();
                    });
                    
                    _onActivateList.Add(currencyObject);
                }

                try
                {
                    await UniTask.WaitForSeconds(0.05f, cancellationToken: _cts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogError($"Failed error {e.Message}");
                    return;
                }
            }

            while (count != spawnCurrencyCount)
            {
                try
                {
                    await UniTask.Yield(_cts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogError($"Failed error {e.Message}");
                    return;
                }
            }
         
            callback?.Invoke();
        }
    }
}