using UnityEngine;

namespace MewVivor.InGame.Enum
{
    public static class Layer
    {
        public static readonly int AttackableLayer = LayerMask.GetMask("Monster", "Boss", "ItemBox");
        public static readonly int MonsterLayer = LayerMask.GetMask("Monster");
        public static readonly int BossLayer = LayerMask.GetMask("Boss");
        public static readonly int Soul = LayerMask.GetMask("Soul");
        public static readonly int Gem = LayerMask.GetMask("ExpGem");
        public static readonly int SafeZone = LayerMask.GetMask("SafeZone");
    }

    public static class Tag
    {
        public static readonly string Monster = "Monster";
        public static readonly string Player = "Player";
        public static readonly string ItemBox = "ItemBox";
    }
}