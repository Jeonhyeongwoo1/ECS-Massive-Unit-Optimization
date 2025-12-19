using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace MewVivor.Util
{
    public static class TaskHelper
    {
        private static UniTaskCompletionSource _tcs;

        public static bool IsTcsTasking()
        {
            return _tcs != null;
        }
        
        public static void InitTcs()
        {
            UI_BlockCanvas.I.ShowAndHideBlockCanvas(true);
            LoadingGenerator.I.ActiveLoadingScreen();
            
            _tcs ??= new UniTaskCompletionSource();
        }

        public static void CompleteTcs()
        {
            if (_tcs == null)
                return;
            var tcs = _tcs;
            _tcs = null;
            tcs.TrySetResult();

            UI_BlockCanvas.I.ShowAndHideBlockCanvas(false);
            LoadingGenerator.I.InActiveLoadingScreen();
        }

        public static void Reset()
        {
            if (_tcs == null)
                return;
            
            var tcs = _tcs;
            _tcs = null;
            tcs?.TrySetCanceled();
            UI_BlockCanvas.I.ShowAndHideBlockCanvas(false);
            LoadingGenerator.I.InActiveLoadingScreen();
        }
    }
}