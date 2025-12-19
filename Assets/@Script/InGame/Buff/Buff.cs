using System.Collections.Generic;
using MewVivor.Enum;
using UnityEngine.Serialization;

namespace MewVivor.InGame.Buff
{
    public struct BuffData
    {
        public DebuffType debuffType;
        public float debuffValue;
        public float debuffValuePercent;
        public float duration;
        public float elapsed;
    }

    public struct ActivateBuffState
    {
        public int BuffCount => BuffDataList?.Count ?? 0;
        public List<BuffData> BuffDataList;
    }
}