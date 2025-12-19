using UnityEngine;

namespace MewVivor
{
    public static class TimeScaleHandler
    {
        private static int _timeScaleCount = 0;

        public static void RequestPause()
        {
            _timeScaleCount++;
            UpdateTimeScale();
        }

        public static void ReleasePause()
        {
            _timeScaleCount--;
            UpdateTimeScale();
        }

        public static void Reset()
        {
            _timeScaleCount = 0;
            UpdateTimeScale();
        }

        private static void UpdateTimeScale()
        {
            Time.timeScale = _timeScaleCount > 0 ? 0 : 1;
        }
    }
}