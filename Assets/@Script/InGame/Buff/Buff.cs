using System;
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

    public struct ActivateBuffState : IEquatable<ActivateBuffState>
    {
        public int BuffCount => BuffDataList?.Count ?? 0;
        public List<BuffData> BuffDataList;

        public bool Equals(ActivateBuffState other)
        {
            return Equals(BuffDataList, other.BuffDataList);
        }

        public override bool Equals(object obj)
        {
            return obj is ActivateBuffState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (BuffDataList != null ? BuffDataList.GetHashCode() : 0);
        }
    }
}