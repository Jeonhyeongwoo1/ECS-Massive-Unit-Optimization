using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using UnityEngine;

namespace MewVivor
{
    public class LoadingGenerator : MonoBehaviour
    {
        private static LoadingGenerator _loadingGenerator;

        public static LoadingGenerator I
        {
            get
            {
                if (_loadingGenerator == null)
                {
                    _loadingGenerator = FindAnyObjectByType<LoadingGenerator>();
                }

                return _loadingGenerator;
            }
        }

        private CancellationTokenSource _cts;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _loadingGenerator = this;
            gameObject.SetActive(false);
        }

        public void ActiveLoadingScreen(bool isImmediate = true)
        {
            if (isImmediate)
            {
                gameObject.SetActive(true);
                return;
            }
            
            ActiveLoadingScreenAsync().Forget();
        }

        private async UniTaskVoid ActiveLoadingScreenAsync()
        {
            try
            {
                await UniTask.WaitForSeconds(1);
                gameObject.SetActive(true);
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                Debug.LogError($"Error {e.Message}");
                InActiveLoadingScreen();
            }
        }

        public void InActiveLoadingScreen()
        {
            Utils.SafeCancelCancellationTokenSource(ref _cts);
            gameObject.SetActive(false);
        }

    }
}