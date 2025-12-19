using System;
using System.Collections.Generic;
using MewVivor.Enum;
using MewVivor.InGame.Enum;
using UnityEngine;
using UnityEngine.Serialization;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public interface ILoader<Value>
{
    List<Value> MakeList();
}

namespace MewVivor.Data
{
    
    #region LevelData
    [Serializable]
    public class LevelData
    {
        public int Level;
        public int AccumulatedEXP;
    }

    [Serializable]
    public class LevelDataLoader : ILoader<int, LevelData>
    {
        public List<LevelData> levels = new List<LevelData>();
        public Dictionary<int, LevelData> MakeDict()
        {
            Dictionary<int, LevelData> dict = new Dictionary<int, LevelData>();
            foreach (LevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    #region AccountLevelData
    [Serializable]
    public class AccountLevelData
    {
        public int Level;
        public long NeedExp;
        public List<int> Reward_ID;
        public List<int> Reward_Amount;
    }

    [Serializable]
    public class AccountLevelDataLoader : ILoader<int, AccountLevelData>
    {
        public List<AccountLevelData> levels = new List<AccountLevelData>();
        public Dictionary<int, AccountLevelData> MakeDict()
        {
            Dictionary<int, AccountLevelData> dict = new Dictionary<int, AccountLevelData>();
            foreach (AccountLevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    
    #region CreatureData

    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float MaxHp;
        public float Atk;
        public float MoveSpeed;
        public float CriticalPercent;
        public float CriticalDamagePercent;
        public float HpRecovery; //초당 회복량
        public string IconLabel;
        public string ResourcePrefabLabel;
        public List<int> SkillTypeList;//InGameSkills를 제외한 추가스킬들
        public float Scale;
    }
    
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new();
        
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creatureData in creatures)
            {
                dict.Add(creatureData.DataId, creatureData);
            }
            return dict;
        }
    }
    
    #endregion
    
    #region SkillData
    [Serializable]
    public class AttackSkillData  : BaseSkillData
    {
        public string SkillSprite;
        public string PrefabLabel; //프리팹 경로
        public SkillCategoryType SkillCategoryType; //스킬 카테고리 타입
        public AttackSkillType AttackSkillType;//스킬 카테고리
        public float ProjectileSpacing;
        public int NumOfProjectile; //갯수
        public float ConeAngle; //각
        public float AttackRange;
        public float Scale;
        public float DamagePercent;
        public float CoolTime;
        public float KnockbackDistance;
        public float ProjectileSpeed;
        public float Efficiency;
        public int EfficiencyPercent;
        public float SkillDuration;
        public float AttackInterval;
        public DebuffType DebuffType1;
        public float DebuffValuePercent1;
        public float DebuffValue1;
        public float DebuffDuration1;
        public DebuffType DebuffType2;
        public float DebuffValuePercent2;
        public float DebuffValue2;
        public float DebuffDuration2;
    }
    
    [Serializable]
    public class AttackSkillDataLoader : ILoader<int, AttackSkillData>
    {
        public List<AttackSkillData> skills = new List<AttackSkillData>();

        public Dictionary<int, AttackSkillData> MakeDict()
        {
            Dictionary<int, AttackSkillData> dict = new Dictionary<int, AttackSkillData>();
            foreach (AttackSkillData skill in skills)
            {
                dict.Add(skill.DataId, skill);
            }
            return dict;
        }
    }
    #endregion
    
    #region StageData
    [Serializable]
    public class StageData
    {
        public int StageIndex = 1;
        public string StageName;
        public int StageLevel = 1;
        public string MapName;
        public string StageImage;
        public float MonsterKillDropScrollprob;
        public int MaxDropScrollCount;
        public float MosterKillDropLevelUpItemProb;
        public string StageNameKey;
        public List<WaveData> WaveList;
    }
    
    public class StageDataLoader : ILoader<int, StageData>
    {
        public List<StageData> stages = new List<StageData>();

        public Dictionary<int, StageData> MakeDict()
        {
            Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
            foreach (StageData stage in stages)
                dict.Add(stage.StageIndex, stage);
            return dict;
        }
    }
    #endregion
    
    #region WaveData
    [Serializable]
    public class WaveData
    {
        public int StageIndex = 1;
        public int WaveIndex = 1;
        public float SpawnInterval = 0.5f;
        public int OnceSpawnCount;
        public List<int> MonsterId;
        public List<int> EliteId;
        public List<int> BossId;
        public float EliteSpawnTime;
        public float AtkRate;
        public float HpRate;
        public float EliteAtkRate;
        public float EliteHpRate;
        public float RemainsTime;
        public WaveType WaveType;
        public float nonDropRate;
        public float BlueGemDropRate;
        public float GreenGemDropRate;
        public float PurpleGemDropRate;
        public float RedGemDropRate;
        public List<int> EliteAndBossClearDropItemId;
        public List<int> EliteAndBossClearDropItemAmount;
        public List<int> WaveClearRewardItemId;
        public List<int> WaveClearRewardItemAmount;
        public int BossClearRewardId;
        public int BossClearRewardMinAmount;
        public int BossClearRewardMaxAmount;
        public float BossClearRewardProb;
    }

    public class WaveDataLoader : ILoader<int, List<WaveData>>
    {
        public List<WaveData> waves = new List<WaveData>();

        public Dictionary<int, List<WaveData>> MakeDict()
        {
            Dictionary<int, List<WaveData>> dict = new Dictionary<int, List<WaveData>>();
            foreach (WaveData wave in waves)
            {
                if (!dict.TryGetValue(wave.StageIndex, out List<WaveData> list))
                {
                    list = new List<WaveData>();
                }
                
                list.Add(wave);
                dict[wave.StageIndex] = list;
            }
            
            return dict;
        }
    }
    #endregion
    
    #region DropItemData
    public class DropItemData
    {
        public int DataId;
        public DropableItemType DropItemType;
        public string NameTextID;
        public float PROB;
        public string DescriptionTextID;
        public string Description;
        public float Value;
        public string SpriteName;
    }
    
    [Serializable]
    public class DropItemDataLoader : ILoader<int, DropItemData>
    {
        public List<DropItemData> DropItems = new List<DropItemData>();
        public Dictionary<int, DropItemData> MakeDict()
        {
            Dictionary<int, DropItemData> dict = new Dictionary<int, DropItemData>();
            foreach (DropItemData dtm in DropItems)
                dict.Add(dtm.DataId, dtm);
            return dict;
        }
    }

    #endregion

    #region ContinueData

    [Serializable]
    public class ContinueData
    {
        public int exp;
    }


    #endregion

    [Serializable]
    public class BaseSkillData
    {
        public int DataId;
        public string TitleTextKey;
        public string DescriptionTextKey;
        public string IconLabel;//아이콘 경로
        public int MatchSkillId;
        public int CurrentLevel; // 스킬 추천용으로 사용
        public SkillType SkillType;
    }
    
    #region SupportSkilllData
    [Serializable]
    public class PassiveSkillData : BaseSkillData
    {
        public PassiveSkillType PassiveSkillType;
        public float Rate;
        public List<int> LinkedSkillId;
    }
    
    [Serializable]
    public class PassiveSkillDataLoader : ILoader<int, PassiveSkillData>
    {
        public List<PassiveSkillData> passiveSkills = new List<PassiveSkillData>();

        public Dictionary<int, PassiveSkillData> MakeDict()
        {
            Dictionary<int, PassiveSkillData> dict = new Dictionary<int, PassiveSkillData>();
            foreach (PassiveSkillData skill in passiveSkills)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
    #endregion
    
    #region MaterialtData
    [Serializable]
    public class ItemData
    {
        public int ID;
        public MaterialType MaterialType;
        public MaterialGrade MaterialGrade;
        public string DescriptionTextID;
        public string SpriteName;
    }

    [Serializable]
    public class ItemDataLoader : ILoader<int, ItemData>
    {
        public List<ItemData> Materials = new List<ItemData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
            foreach (ItemData mat in Materials)
            {
                dict.Add(mat.ID, mat);
            }
            return dict;
        }
    }
    #endregion

    #region DefaultUserData

    [Serializable]
    public class DefaultUserData
    {
        public int itemId;
        public string itemName;
        public int itemValue;
    }

    [Serializable]
    public class DefaultUserDataLoader : ILoader<int, DefaultUserData>
    {
        public List<DefaultUserData> defaultUserDataList = new ();

        public Dictionary<int, DefaultUserData> MakeDict()
        {
            Dictionary<int, DefaultUserData> dict = new ();
            foreach (DefaultUserData item in defaultUserDataList)
                dict.Add(item.itemId, item);
            return dict;
        }
    }

    #endregion
 
    #region EquipmentData
    [Serializable]
    public class EquipmentData
    {
        public int ItemCode;
        public string DropProbId;
        public EquipmentType EquipmentType;
        public EquipmentGrade Grade;
        public string NameTextID;
        public string DescriptionTextID;
        public string Sprite;
        public int Grade_Hp;
        public int LevelUp_HP;
        public int Grade_Atk;
        public int LevelUp_Atk;
        public int Grade_MaxLevel;
        public int LevelupMaterialID;
        public MergeType MergeType;
        public int MergeNeedCount;
        public int SkillGroupDataId;
        public int MergeSuccessItemCode;
    }

    [Serializable]
    public class EquipmentDataLoader : ILoader<int, EquipmentData>
    {
        public List<EquipmentData> Equipments = new List<EquipmentData>();
        public Dictionary<int, EquipmentData> MakeDict()
        {
            Dictionary<int, EquipmentData> dict = new Dictionary<int, EquipmentData>();
            foreach (EquipmentData equip in Equipments)
                dict.Add(equip.ItemCode, equip);
            return dict;
        }
    }
    #endregion

    #region EquipmentDropData
    [Serializable]
    public class EquipmentDropData
    {
        public string ProbId;
        public EquipmentGrade Grade;
        public EquipmentType EquipmentType;
        public float SilverKeyDropProb;
        public float GoldenKeyDropProb;
    }

    [Serializable]
    public class EquipmentDropDataLoader : ILoader<string, EquipmentDropData>
    {
        public List<EquipmentDropData> Equipments = new List<EquipmentDropData>();
        public Dictionary<string, EquipmentDropData> MakeDict()
        {
            Dictionary<string, EquipmentDropData> dict = new Dictionary<string, EquipmentDropData>();
            foreach (EquipmentDropData equip in Equipments)
                dict.Add(equip.ProbId, equip);
            return dict;
        }
    }
    
    #endregion
    
    #region LevelData
    [Serializable]
    public class EquipmentLevelData
    {
        public int Level;
        public int UpgradeCost;
        public int UpgradeRequiredItems;
    }

    [Serializable]
    public class EquipmentLevelDataLoader : ILoader<int, EquipmentLevelData>
    {
        public List<EquipmentLevelData> levels = new List<EquipmentLevelData>();
        public Dictionary<int, EquipmentLevelData> MakeDict()
        {
            Dictionary<int, EquipmentLevelData> dict = new Dictionary<int, EquipmentLevelData>();

            foreach (EquipmentLevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    #region EquipmentSkillData
    [Serializable]
    public class EquipmentSkillData
    {
        public int DataId;
        public string DescriptionTextID;
        public EquipmentSkillType EquipmentSkillType;
        public float Type_Value1;
        public float Type_Value2;
        public float Type_Value3;
    }

    [Serializable]
    public class EquipmentSkillDataLoader : ILoader<int, EquipmentSkillData>
    {
        public List<EquipmentSkillData> levels = new();
        public Dictionary<int, EquipmentSkillData> MakeDict()
        {
            Dictionary<int, EquipmentSkillData> dict = new Dictionary<int, EquipmentSkillData>();

            foreach (EquipmentSkillData levelData in levels)
                dict.Add(levelData.DataId, levelData);
            return dict;
        }
    }
    #endregion

    #region EquipmentSkillGroupData

    [Serializable]
    public class EquipmentSkillGroupData
    {
        public int DataId;
        public List<int> ItemCode;
        public int UncommonGradeSkill;
        public int RareGradeSkill;
        public int EpicGradeSkill;
        public int LegendaryGradeSkill;
    }

    [Serializable]
    public class EquipmentSkillGroupDataLoader : ILoader<int, EquipmentSkillGroupData>
    {
        public List<EquipmentSkillGroupData> SkillGroupDatas = new();
        public Dictionary<int, EquipmentSkillGroupData> MakeDict()
        {
            Dictionary<int, EquipmentSkillGroupData> dict = new Dictionary<int, EquipmentSkillGroupData>();

            foreach (EquipmentSkillGroupData skillGroupData in SkillGroupDatas)
                dict.Add(skillGroupData.DataId, skillGroupData);
            return dict;
        }
    }
    #endregion

    #region ShopData
    [Serializable]
    public class ShopData
    {
        public string Id;
        public int SlotIndex;
        public ShopGroupType ShopGroupType;
        public string Name; //상품 이름
        public PayType PayType;
        public List<int> CostId;
        public int Efficient;
        public List<int> CostPrice;
        public float USD;
        public ShopIAPIdType StoreProductId;
        public int count;
        public BuyLimitType BuyLimitType;
        public int BuyLimitCount;
        public List<int> Reward_ID;
        public List<int> Reward_Amount;
        public bool IsFirstDouble;
    }

    [Serializable]
    public class ShopDataDataLoader : ILoader<string, ShopData>
    {
        public List<ShopData> shops = new List<ShopData>();
        public Dictionary<string, ShopData> MakeDict()
        {
            Dictionary<string, ShopData> dict = new Dictionary<string, ShopData>();

            foreach (ShopData shopData in shops)
                dict.Add(shopData.Id, shopData);
            return dict;
        }
    }
    
    #endregion
    
    #region CheckOutData
    public class CheckOutData
    {
        public int Day;
        public int RewardItemId;
        public int MissionTarRewardItemValuegetValue;
    }

    [Serializable]
    public class CheckOutDataLoader : ILoader<int, CheckOutData>
    {
        public List<CheckOutData> checkouts = new List<CheckOutData>();
        public Dictionary<int, CheckOutData> MakeDict()
        {
            Dictionary<int, CheckOutData> dict = new Dictionary<int, CheckOutData>();
            foreach (CheckOutData chk in checkouts)
                dict.Add(chk.Day, chk);
            return dict;
        }
    }
    #endregion
    
    
    #region MissionData
    public class MissionData
    {
        public int MissionId;
        public MissionType MissionType;
        public string DescriptionTextID;
        public MissionTarget MissionTarget;
        public int MissionTargetValue;
        public int ClearRewardItmeId;
        public int RewardValue;
    }

    [Serializable]
    public class MissionDataLoader : ILoader<int, MissionData>
    {
        public List<MissionData> missions = new List<MissionData>();
        public Dictionary<int, MissionData> MakeDict()
        {
            Dictionary<int, MissionData> dict = new Dictionary<int, MissionData>();
            foreach (MissionData mis in missions)
                dict.Add(mis.MissionId, mis);
            return dict;
        }
    }
    #endregion
    
    #region AchievementData
    [Serializable]
    public class AchievementData
    {
        public int AchievementMissionID;
        public string DescriptionTextKey;
        public AchievementMissionTarget AchievementMissionTarget;
        public int AchievementMissionDefaultGoal;
        public int AchievementMissionIncreaseGoal;
        public int AchievementMissionMaxLevel;
        public int Reward_Id;
        public int Reward_Amount;
    }

    [Serializable]
    public class AchievementDataLoader : ILoader<int, AchievementData>
    {
        public List<AchievementData> Achievements = new List<AchievementData>();
        public Dictionary<int, AchievementData> MakeDict()
        {
            Dictionary<int, AchievementData> dict = new Dictionary<int, AchievementData>();
            foreach (AchievementData ach in Achievements)
                dict.Add(ach.AchievementMissionID, ach);
            return dict;
        }
    }
    #endregion
    
    #region OfflineRewardData
    
    [Serializable]
    public class OfflineRewardData
    {
        public int StageIndex;
        public int Reward_Gold;
        public int Reward_Exp;
        public int FastReward_Scroll;
        public int FastReward_Box;
    }

    [Serializable]
    public class OfflineRewardDataLoader : ILoader<int, OfflineRewardData>
    {
        public List<OfflineRewardData> offlines = new List<OfflineRewardData>();
        public Dictionary<int, OfflineRewardData> MakeDict()
        {
            Dictionary<int, OfflineRewardData> dict = new Dictionary<int, OfflineRewardData>();
            foreach (OfflineRewardData ofr in offlines)
                dict.Add(ofr.StageIndex, ofr);
            return dict;
        }
    }
    #endregion

    #region GlobalConfig

    [Serializable]
    public class GlobalConfigData
    {
        public string Name;
        public float Value;
        public string Description;
    }
    
    [Serializable]
    public class GlobalConfigDataLoader : ILoader<GlobalConfigName, GlobalConfigData>
    {
        public List<GlobalConfigData> globalConfigs = new List<GlobalConfigData>();
        public Dictionary<GlobalConfigName, GlobalConfigData> MakeDict()
        {
            Dictionary<GlobalConfigName, GlobalConfigData> dict = new();
            foreach (GlobalConfigData ofr in globalConfigs)
            {
                dict.Add((GlobalConfigName)System.Enum.Parse(typeof(GlobalConfigName), ofr.Name), ofr);
            }
            return dict;
        }
    }
    
    #endregion

    #region HuntPassConfig

    
    [Serializable]
    public class HuntPassConfigData
    {
        public string Name;
        public string Value;
        public string Description;
    }
    
    [Serializable]
    public class HuntPassConfigDataLoader : ILoader<HuntPassConfigName, HuntPassConfigData>
    {
        public List<HuntPassConfigData> huntPassConfigs = new List<HuntPassConfigData>();
        public Dictionary<HuntPassConfigName, HuntPassConfigData> MakeDict()
        {
            Dictionary<HuntPassConfigName, HuntPassConfigData> dict = new();
            foreach (HuntPassConfigData ofr in huntPassConfigs)
            {
                dict.Add((HuntPassConfigName)System.Enum.Parse(typeof(HuntPassConfigName), ofr.Name), ofr);
            }
            return dict;
        }
    }

#endregion
    
    #region  InfiniteModeConfig

    [Serializable]
    public class InfiniteModeConfigData
    {
        public string Name;
        public float Value;
        public string Description;
    }
    
    [Serializable]
    public class InfiniteModeConfigDataLoader : ILoader<InfiniteModeConfigName,InfiniteModeConfigData>
    {
        public List<InfiniteModeConfigData> infiniteModeConfig = new List<InfiniteModeConfigData>();
        public Dictionary<InfiniteModeConfigName, InfiniteModeConfigData> MakeDict()
        {
            Dictionary<InfiniteModeConfigName, InfiniteModeConfigData> dict = new();
            foreach (InfiniteModeConfigData ofr in infiniteModeConfig)
                dict.Add((InfiniteModeConfigName)System.Enum.Parse(typeof(InfiniteModeConfigName), ofr.Name), ofr);
            return dict;
        }
    }
    
    #endregion
    
        
    #region Localization

    [Serializable]
    public class LocalizationData
    {
        public string TextKey;
        public string Kor;
        public string Eng;

        public string GetValueByLanguage()
        {
            switch (Manager.I.LanguageType)
            {
                case LanguageType.Eng:
                    return Eng;
                case LanguageType.Kor:
                    return Kor;
                default:
                    return Eng;
            }
        }
    }
    
    [Serializable]
    public class LocalizationDataLoader : ILoader<string , LocalizationData>
    {
        public List<LocalizationData> localizationDataList = new List<LocalizationData>();
        
        public Dictionary<string, LocalizationData> MakeDict()
        {
            Dictionary<string, LocalizationData> dict = new();
            foreach (LocalizationData data in localizationDataList)
            {
                dict.Add(data.TextKey, data);
            }
            return dict;
        }
    }
    
    #endregion

    #region Evolution

    [Serializable]
    public class EvolutionData
    {
        public string DataId;
        public string TitleTextKey;
        public string DescriptionTextKey;
        public EvolutionGrade Grade;
        public EvolutionType EvolutionType;
        public float DefaultEvolutionValue1;
        public float DefaultEvolutionValue2;
        public float LevelUpEvolutionValue1;
        public float LevelUpEvolutionValue2;
        public int MaxCount;

        public float GetEvolutionValue1(int level)
        {
            switch (EvolutionType)
            {
                case EvolutionType.Atk:
                case EvolutionType.MaxHp:
                case EvolutionType.invincibility:
                case EvolutionType.Boom:
                case EvolutionType.Berserker:
                    //level 1 -> 0으로 
                    return DefaultEvolutionValue1 + LevelUpEvolutionValue1 * (level - 1);
                case EvolutionType.MoveSpeed:
                case EvolutionType.CriticalPercent:
                case EvolutionType.CriticalDamage:
                case EvolutionType.SkillCoolDown:
                    //level 1 -> 0으로 
                    return (DefaultEvolutionValue1 + LevelUpEvolutionValue1 * (level - 1)) * 100;
            }

            Debug.LogError($"Failed error {level} / Type {EvolutionType}");
            return 0;
        }

        public float GetEvolutionValue2(int level)
        {
            switch (EvolutionType)
            {
                case EvolutionType.Atk:
                case EvolutionType.MaxHp:
                case EvolutionType.invincibility:
                case EvolutionType.Boom:
                    //level 1 -> 0으로 
                    return DefaultEvolutionValue2 + LevelUpEvolutionValue2 * (level - 1);
                case EvolutionType.MoveSpeed:
                case EvolutionType.CriticalPercent:
                case EvolutionType.CriticalDamage:
                case EvolutionType.SkillCoolDown:
                case EvolutionType.Berserker:
                    //level 1 -> 0으로 
                    return (DefaultEvolutionValue2 + LevelUpEvolutionValue2 * (level - 1)) * 100;
            }

            Debug.LogError($"Failed error {level} / Type {EvolutionType}");
            return 0;
        }

        public string GetDescription(int level)
        {
            string result = "";
            LocalizationData data = null;
            switch (EvolutionType)
            {
                case EvolutionType.Atk:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level));
                    break;
                case EvolutionType.MaxHp:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level));
                    break;
                case EvolutionType.MoveSpeed:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level));
                    break;
                case EvolutionType.CriticalPercent:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level));
                    break;
                case EvolutionType.CriticalDamage:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level));
                    break;
                case EvolutionType.Boom:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue2(level));
                    break;
                case EvolutionType.SkillCoolDown:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level));
                    break;
                case EvolutionType.Berserker:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue1(level),
                        GetEvolutionValue2(level));
                    break;
                case EvolutionType.invincibility:
                    data = Manager.I.Data.LocalizationDataDict[DescriptionTextKey];
                    result = string.Format(data.GetValueByLanguage(), GetEvolutionValue2(level), GetEvolutionValue1(level));
                    break;
            }

            return result;
        }
    }
    
    [Serializable]
    public class EvolutionDataLoader : ILoader<string , EvolutionData>
    {
        public List<EvolutionData> evolutionDataList = new List<EvolutionData>();
        
        public Dictionary<string, EvolutionData> MakeDict()
        {
            Dictionary<string, EvolutionData> dict = new();
            foreach (EvolutionData data in evolutionDataList)
            {
                dict.Add(data.DataId, data);
            }
            return dict;
        }
    }
    
    #endregion
    
    #region EvolutionOrder

    [Serializable]
    public class EvolutionOrderData
    {
        public int Level;
        public EvolutionOrderType OrderTypeA;
        public string DescriptionA;
        public EvolutionOrderType OrderTypeB;
        public string DescriptionB;
        public EvolutionOrderType OrderTypeC;
        public string DescriptionC;
        public EvolutionOrderType OrderTypeD;
        public string DescriptionD;
        public EvolutionOrderType OrderTypeE;
        public string DescriptionE;
    }
    
    [Serializable]
    public class EvolutionOrderLoader : ILoader<int , EvolutionOrderData>
    {
        public List<EvolutionOrderData> evolutionOrderDataList = new();
        public Dictionary<int, EvolutionOrderData> MakeDict()
        {
            Dictionary<int, EvolutionOrderData> dict = new();
            foreach (EvolutionOrderData data in evolutionOrderDataList)
            {
                dict.Add(data.Level, data);
            }
            return dict;
        }
    }
    
    #endregion

    #region Quest
    
    [Serializable]
    public class QuestData
    {
        public int QuestMissionId;
        public QuestType QuestMissionTyped;
        public string DescriptionTextKey;
        public QuestMissionTarget QuestMissionTarget;
        public int QuestMissionTargetValue;
        public int Reward_Id;
        public int Reward_Amount;
    }
    
    [Serializable]
    public class QuestDataLoader : ILoader<int , QuestData>
    {
        public List<QuestData> questDataList = new();
        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dict = new();
            foreach (QuestData data in questDataList)
            {
                dict.Add(data.QuestMissionId, data);
            }
            return dict;
        }
    }
    
    #endregion

    #region DailyQuestReward

    [Serializable]
    public class DailyQuestRewardData
    {
        public int IDX;
        public int NeedDailyPointReward_Id;
        public int Reward_Id;
        public int Reward_Amount;
    }
    
    [Serializable]
    public class DailyQuestRewardDataLoader : ILoader<int , DailyQuestRewardData>
    {
        public List<DailyQuestRewardData> questDataList = new();
        public Dictionary<int, DailyQuestRewardData> MakeDict()
        {
            Dictionary<int, DailyQuestRewardData> dict = new();
            foreach (DailyQuestRewardData data in questDataList)
            {
                dict.Add(data.IDX, data);
            }
            
            return dict;
        }
    }
    
    #endregion

    #region PassReward

    [Serializable]
    public class PassRewardData
    {
        public int Id;
        public int Level;
        public PassType PassType;
        public int NeedPoint;
        public int FreeReward_Id;
        public int FreeReward_Amount;
        public int PaidReward_Id;
        public int PaidReward_Amount;
    }
    
    [Serializable]
    public class PassRewardDataLoader : ILoader<int , PassRewardData>
    {
        public List<PassRewardData> passRewardDataList = new();
        public Dictionary<int, PassRewardData> MakeDict()
        {
            Dictionary<int, PassRewardData> dict = new();
            foreach (PassRewardData data in passRewardDataList)
            {
                dict.Add(data.Id, data);
            }
            
            return dict;
        }
    }
    
    #endregion

    #region ShopSubscribe
    

    [Serializable]
    public class ShopSubscribeData
    {
        public string Id;
        public int DurationDays;
        public int Reward_ID;
        public int Reward_Amount;
    }
    
    [Serializable]
    public class ShopSubscribeDataLoader : ILoader<string , ShopSubscribeData>
    {
        public List<ShopSubscribeData> shopSubscribeDataList = new();
        public Dictionary<string, ShopSubscribeData> MakeDict()
        {
            Dictionary<string, ShopSubscribeData> dict = new();
            foreach (ShopSubscribeData data in shopSubscribeDataList)
            {
                dict.Add(data.Id, data);
            }
            
            return dict;
        }
    }

    #endregion

    #region Banword
    
    [Serializable]
    public class BanWordDataLoader : ILoader<string>
    {
        public List<string> banWordList = new();
        public List<string> MakeList()
        {
            List<string> list = new List<string>();
            foreach (string data in banWordList)
            {
                list.Add(data);
            }

            return list;
        }
    }
    #endregion
    
}