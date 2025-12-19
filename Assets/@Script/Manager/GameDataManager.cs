using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using UnityEngine;
using EquipmentLevelData = MewVivor.Data.EquipmentLevelData;
using ShopData = MewVivor.Data.ShopData;

namespace MewVivor.Managers
{
    public class DataManager
    {
        public Dictionary<int, AttackSkillData> AttackSkillDict { get; private set; } = new();
        public Dictionary<int, CreatureData> CreatureDict { get; private set; } = new();
        public Dictionary<int, StageData> StageDict { get; private set; } = new();
        public Dictionary<int, LevelData> LevelDataDict { get; private set; } = new();
        public Dictionary<int, DropItemData> DropItemDict { get; private set; } = new();
        public Dictionary<int, PassiveSkillData> PassiveSkillDataDict { get; private set; } = new();
        public Dictionary<int, ItemData> ItemDataDict { get; private set; } = new();
        public Dictionary<int, DefaultUserData> DefaultUserDataDict { get; private set; } = new();
        public Dictionary<int, EquipmentData> EquipmentDataDict { get; private set; } = new();
        public Dictionary<int, EquipmentLevelData> EquipmentLevelDataDict { get; private set; } = new();
        public Dictionary<string, ShopData> ShopDataDict { get; private set; } = new();
        public Dictionary<int, CheckOutData> CheckOutDataDict { get; private set; } = new();
        public Dictionary<int, MissionData> MissionDataDict { get; private set; } = new();
        public Dictionary<int, AchievementData> AchievementDataDict { get; private set; } = new();
        public Dictionary<int, OfflineRewardData> OfflineRewardDataDict { get; private set; } = new();
        public Dictionary<GlobalConfigName, GlobalConfigData> GlobalConfigDataDict { get; private set; } = new();
        public Dictionary<InfiniteModeConfigName, InfiniteModeConfigData> InfiniteModeConfigDataDict { get; private set; } = new();
        public Dictionary<string, LocalizationData> LocalizationDataDict { get; private set; } = new();
        public Dictionary<int, EquipmentSkillData> EquipmentSkillDataDict { get; private set; } = new();
        public Dictionary<int, EquipmentSkillGroupData> EquipmentSkillGroupDataDict { get; private set; } = new();
        public Dictionary<string, EvolutionData> EvolutionDataDict { get; private set; } = new();
        public Dictionary<int, EvolutionOrderData> EvolutionOrderDataDict { get; private set; } = new(); 
        public Dictionary<int, AccountLevelData> AccountLevelDict { get; private set; } = new();
        public Dictionary<int, QuestData> QuestDataDict { get; private set; } = new();
        public Dictionary<int, DailyQuestRewardData> DailyQuestRewardDataDict { get; private set; } = new();
        public Dictionary<int, PassRewardData> PassRewardDataDict { get; private set; }
        public Dictionary<string, ShopSubscribeData> ShopSubscribeDataDict { get; private set; }
        public Dictionary<HuntPassConfigName, HuntPassConfigData> HuntPassConfigDataDict { get; private set; }
        public Dictionary<string, EquipmentDropData> EquipmentDropDataDict { get; private set; }
        
        public List<string> BanWordDataList { get; private set; }
        
        public void Initialize()
        {
            AttackSkillDict = LoadJson<AttackSkillDataLoader, int, AttackSkillData>("AttackSkillData").MakeDict();
            CreatureDict = LoadJson<CreatureDataLoader, int, CreatureData>("CreatureData").MakeDict();
            StageDict = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
            LevelDataDict = LoadJson<LevelDataLoader, int, LevelData>("LevelData").MakeDict();
            DropItemDict = LoadJson<DropItemDataLoader, int, DropItemData>("DropItemData").MakeDict();
            PassiveSkillDataDict =
                LoadJson<PassiveSkillDataLoader, int, PassiveSkillData>("PassiveSkillData").MakeDict();
            ItemDataDict = LoadJson<ItemDataLoader, int, ItemData>("ItemData").MakeDict();
            DefaultUserDataDict = LoadJson<DefaultUserDataLoader, int, DefaultUserData>("DefaultUserData").MakeDict();
            EquipmentDataDict = LoadJson<EquipmentDataLoader, int, EquipmentData>("EquipmentData").MakeDict();
            EquipmentLevelDataDict =
                LoadJson<EquipmentLevelDataLoader, int, EquipmentLevelData>("EquipmentLevelData").MakeDict();
            
            ShopDataDict = LoadJson<ShopDataDataLoader, string, ShopData>("ShopData").MakeDict();
            CheckOutDataDict = LoadJson<CheckOutDataLoader, int, CheckOutData>("CheckOutData").MakeDict();
            MissionDataDict = LoadJson<MissionDataLoader, int, MissionData>("MissionData").MakeDict();
            AchievementDataDict = LoadJson<AchievementDataLoader, int, AchievementData>("AchievementData").MakeDict();
            OfflineRewardDataDict = LoadJson<OfflineRewardDataLoader, int, OfflineRewardData>("OfflineRewardData")
                .MakeDict();
            GlobalConfigDataDict = LoadJson<GlobalConfigDataLoader, GlobalConfigName, GlobalConfigData>("GlobalConfigData")
                .MakeDict();
            InfiniteModeConfigDataDict =
                LoadJson<InfiniteModeConfigDataLoader, InfiniteModeConfigName, InfiniteModeConfigData>("InfiniteModeConfigData").MakeDict();
            LocalizationDataDict = LoadJson<LocalizationDataLoader, string, LocalizationData>("LocalizationData").MakeDict();
            EquipmentSkillDataDict = LoadJson<EquipmentSkillDataLoader, int, EquipmentSkillData>("EquipmentSkillData")
                .MakeDict();
            EquipmentSkillGroupDataDict = LoadJson<EquipmentSkillGroupDataLoader, int, EquipmentSkillGroupData>("EquipmentSkillGroupData").MakeDict();
            EvolutionDataDict = LoadJson<EvolutionDataLoader, string, EvolutionData>("EvolutionData").MakeDict();
            AccountLevelDict = LoadJson<AccountLevelDataLoader, int, AccountLevelData>("AccountLevelData").MakeDict();
            QuestDataDict = LoadJson<QuestDataLoader, int, QuestData>("QuestData").MakeDict();
            DailyQuestRewardDataDict = LoadJson<DailyQuestRewardDataLoader, int, DailyQuestRewardData>("DailyQuestRewardData").MakeDict();
            PassRewardDataDict = LoadJson<PassRewardDataLoader, int, PassRewardData>("PassRewardData").MakeDict();
            ShopSubscribeDataDict = LoadJson<ShopSubscribeDataLoader, string, ShopSubscribeData>("ShopSubscribeData")
                .MakeDict();
            HuntPassConfigDataDict =
                LoadJson<HuntPassConfigDataLoader, HuntPassConfigName, HuntPassConfigData>("HuntPassConfigData")
                    .MakeDict();
            EquipmentDropDataDict = LoadJson<EquipmentDropDataLoader, string, EquipmentDropData>("EquipmentDropData")
                .MakeDict();
            Dictionary<int, List<WaveData>> waveDataDict = LoadJson<WaveDataLoader, int, List<WaveData>>("WaveData").MakeDict();
            foreach (var (key, value) in StageDict)
            {
                if (!waveDataDict.ContainsKey(key))
                {
                    continue;
                }

                value.WaveList = waveDataDict[key];
            }

            BanWordDataList = LoadJson<BanWordDataLoader, string>("BanWordData").MakeList();
        }

        TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
        {
            TextAsset textAsset = Manager.I.Resource.Load<TextAsset>($"{path}");
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }

        TLoader LoadJson<TLoader, TValue>(string path) where TLoader : ILoader<TValue>
        {
            TextAsset textAsset = Manager.I.Resource.Load<TextAsset>($"{path}");
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }
    }
}