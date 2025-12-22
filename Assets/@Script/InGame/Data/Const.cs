using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Enum;
using UnityEngine;

namespace MewVivor.Data
{
    public class Const
    {
        public static readonly string PATH = Application.persistentDataPath + "/SaveData.json";
        public const int PLAYER_DATA_ID = 201000;
        public const int MAX_AttackSKiLL_Level = 6;
        public const int MAX_PassiveSkill_Level = 5;
        public const int MAX_SKILL_COUNT = 6;
        public const int ITEM_BOX_DATA_ID = 60001;
        public const float WALL_COLLISION_TAKE_DAMAGE = 5f;
        public const int Nickname_change_price = 100;
      
        #region PoolId

        public const string ExpGem = "ExpGem";
        public const string Revival = "RevivePtcl";

        #endregion
        
        public static float DEFAULT_MagneticRange = 2.25f;
        
        #region 데이터아이디
        public static int ID_GOLD = 50001;
        public static int ID_JEWEL = 50002;
        public static int ID_STAMINA = 50003;
        public static int ID_BRONZE_KEY = 50201;
        public static int ID_SILVER_KEY = 50202;
        public static int ID_GOLD_KEY = 50203;
        public static int ID_RANDOM_SCROLL = 50301;
        public static int ID_POTION = 60001;
        public static int ID_MAGENTIC = 60002;
        public static int ID_SKILLUP = 60007;
        public static int ID_SMALL_BOMB = 60003;
        public static int ID_NORMAL_BOMB = 60004;
        public static int ID_GOLD_10 = 60008;
        public static int ID_GOLD_50 = 60009;
        public static int ID_GOLD_100 = 60010;
        public static int ID_GOLD_300 = 60011;
        public static int ID_JEWEL_1 = 60012;
        public static int ID_JEWEL_5 = 60013;
        public static int ID_JEWEL_10 = 60014;
        public static int ID_INFINITE_TICKET = 50401;

        public static int ID_WEAPON_SCROLL = 50101;
        public static int ID_GLOVES_SCROLL = 50102;
        public static int ID_RING_SCROLL = 50103;
        public static int ID_BELT_SCROLL = 50104;
        public static int ID_ARMOR_SCROLL = 50105;
        public static int ID_BOOTS_SCROLL = 50106;

        #endregion
        
        public static string HourPerName = "시간당";
        public static int MIN_OFFLINE_REWARD_MINUTE = 10;
        
        public static float MonsterAttackIntervalTime = .1f; //몬스터 공격 시간
        public static float PlayerInvincibilityTime = 3f; //플레이어 무적 시간
        public static int ResurrectionRemainTime = 10;
        public static float KNOCKBACK_TIME = 0.3f;// 밀려나는시간
        public static float KNOCKBACK_SPEED = 40;  // 속도 
        public static float KNOCKBACK_COOLTIME = 0.5f;
        public static float PROJECTILE_LIFE_CYCLE = 3f;
        public static int SkillSelectRefreshMaxCount = 4;
        public static int ResurrectionAvailableCount = 1;
        public static int DropItemMaxCount = 900;
        public static int MaxOnceSpawnNormalMonsterCount = 200;
        public static int ReviveSkillId = 116003;
        public static int AddReviveSkillId = 116004;
        public static int InfinityTickCountForStartGame = 1;
        public static int MaxShopNormalGachaAdsCount = 2;
        
        public static string UltimateSkillIconPrefix = "Skill_Ultimate_Icon_";
        
        public static readonly Color ActivateAttackSkillColor = Utils.HexToColor("D88F00");
        public static readonly Color DeActivateAttackSkillColor = Utils.HexToColor("9E9E9E");
        public static readonly Color ActivatePassiveSkillColor = Utils.HexToColor("4F9200");
        public static readonly Color DeActivatePassiveSkillColor = Utils.HexToColor("9E9E9E");
        public static readonly Color UltimateSkillColor = Utils.HexToColor("D81700");
        public static readonly Color MergeSortTypeDeactiveColor = Utils.HexToColor("A9AAAB");
        public static readonly Color MergeSortTypeActiveColor = Color.white;


        public static string GoldSpriteName = "0_CoinIcon.sprite";
        public static string BundleGoldSpriteName = "Gold_Icon.sprite";
        public static string JewelSpriteName = "Jewel_Default.sprite";
        public static string AdsSpriteName = "Icon_Ads.sprite";
        public static string Shop_RewardBox_Gold_SpriteName = "Shop_RewardBox_Gold.sprite";
        public static string Shop_RewardBox_Jewel_SpriteName = "Shop_RewardBox_Jewel.sprite";
        public static string Shop_RewardBox_Scroll_SpriteName = "Shop_RewardBox_Scroll.sprite";
        public static string EquipmentItem_Default_BG_Sprite = "Common.sprite";

        public static string Scroll_Armor_Icon_SpriteName = "Scroll_Armor_Icon.sprite";
        public static string Scroll_Belt_Icon_SpriteName = "Scroll_Belt_Icon.sprite";
        public static string Scroll_Boots_Icon_SpriteName = "Scroll_Boots_Icon.sprite";
        public static string Scroll_Gloves_Icon_SpriteName = "Scroll_Gloves_Icon.sprite";
        public static string Scroll_Ring_Icon_SpriteName = "Scroll_Ring_Icon.sprite";
        public static string Scroll_Weapon_Icon_SpriteName = "Scroll_Weapon_Icon.sprite";
        public static string INFINITE_Icon_SpriteName = "Endless_Ticket.sprite";
        public static string QuestExp_Icon_SpriteName = "QuestExp_Icon.sprite";
        public static string Small_Bomb_Effect_PrefabName = "Explosion_Small";
        public static string Normal_Bomb_Effect_PrefabName = "Explosion_Normal";

        public static int SortinglayerOrder_Monster = 10;
        public static int SortinglayerOrder_Gem = 5;

        public static string Explosion_Name = "Explosion";
        
        public static class EquipmentUIColors
        {
            #region 장비 이름 색상
            public static readonly Color CommonNameColor = Utils.HexToColor("A2A2A2");
            public static readonly Color UncommonNameColor = Utils.HexToColor("57FF0B");
            public static readonly Color RareNameColor = Utils.HexToColor("2471E0");
            public static readonly Color EpicNameColor = Utils.HexToColor("9F37F2");
            public static readonly Color LegendaryNameColor = Utils.HexToColor("F67B09");
            public static readonly Color MythNameColor = Utils.HexToColor("F1331A");
            #endregion
            #region 테두리 색상
            public static readonly Color Common = Utils.HexToColor("ABB4C8");
            public static readonly Color Uncommon = Utils.HexToColor("7EB126");
            public static readonly Color Rare = Utils.HexToColor("4F74C6");
            public static readonly Color Epic = Utils.HexToColor("AA32F5");
            public static readonly Color Legendary = Utils.HexToColor("E5BB10");
            public static readonly Color Myth = Utils.HexToColor("C94B2B");

            public static Color GetMaterialGradeColor(MaterialGrade materialGrade)
            {
                switch (materialGrade)
                {
                    case MaterialGrade.Common:
                        return Common;
                    case MaterialGrade.Uncommon:
                        return Uncommon;
                    case MaterialGrade.Rare:
                        return Rare;
                    case MaterialGrade.Epic:
                    case MaterialGrade.Epic1:
                    case MaterialGrade.Epic2:
                        return Epic;
                    case MaterialGrade.Legendary:
                    case MaterialGrade.Legendary1:
                    case MaterialGrade.Legendary2:
                    case MaterialGrade.Legendary3:
                        return Legendary;
                }

                Debug.LogError($"Failed {nameof(GetEquipmentGradeColor)} / grade {materialGrade}");
                return Color.white;
            }
            
            public static Color GetEquipmentGradeColor(EquipmentGrade equipmentGrade)
            {
                switch (equipmentGrade)
                {
                    case EquipmentGrade.Common:
                        return Common;
                    case EquipmentGrade.Uncommon:
                        return Uncommon;
                    case EquipmentGrade.Rare:
                        return Rare;
                    case EquipmentGrade.Epic:
                    case EquipmentGrade.Epic1:
                    case EquipmentGrade.Epic2:
                        return Epic;
                    case EquipmentGrade.Legendary:
                    case EquipmentGrade.Legendary1:
                    case EquipmentGrade.Legendary2:
                    case EquipmentGrade.Legendary3:
                        return Legendary;
                    case EquipmentGrade.Myth:
                        return Myth;
                }

                Debug.LogError($"Failed {nameof(GetEquipmentGradeColor)} / grade {equipmentGrade}");
                return Color.white;
            }
            
            public static Sprite GetEquipmentGradeSprite(EquipmentGrade equipmentGrade)
            {
                Sprite sprite = null;
                switch (equipmentGrade)
                {
                    case EquipmentGrade.Common:
                        return Manager.I.Resource.Load<Sprite>("Common.sprite");
                    case EquipmentGrade.Uncommon:
                        return Manager.I.Resource.Load<Sprite>("UnCommon.sprite");
                    case EquipmentGrade.Rare:
                        return Manager.I.Resource.Load<Sprite>("Rare.sprite");
                    case EquipmentGrade.Epic:
                    case EquipmentGrade.Epic1:
                    case EquipmentGrade.Epic2:
                        return Manager.I.Resource.Load<Sprite>("Epic.sprite");
                    case EquipmentGrade.Legendary:
                    case EquipmentGrade.Legendary1:
                    case EquipmentGrade.Legendary2:
                    case EquipmentGrade.Legendary3:
                        return Manager.I.Resource.Load<Sprite>("Legend.sprite");
                    case EquipmentGrade.Myth:
                        return Manager.I.Resource.Load<Sprite>("Myth.sprite");
                }

                Debug.LogError($"Failed {nameof(GetEquipmentGradeColor)} / grade {equipmentGrade}");
                return Manager.I.Resource.Load<Sprite>("Common.sprite");
            }
            
            #endregion
            #region 배경색상
            public static readonly Color EpicBg = Utils.HexToColor("D094FF");
            public static readonly Color LegendaryBg = Utils.HexToColor("F8BE56");
            public static readonly Color MythBg = Utils.HexToColor("FF7F6E");
            #endregion
        }
        
        
        public static class AnimationName
        {
            public static int Walk = Animator.StringToHash("walk");
            public static int Idle = Animator.StringToHash("idle");
            public static int Die = Animator.StringToHash("die");
            public static int Attack = Animator.StringToHash("attack");
        }
    }
}