using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using MewVivor.Data;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/ParseExcelSafe")]
    public static void ParseExcelSafe()
    {
        ParseExcelDataToJson<AttackSkillDataLoader, AttackSkillData>("AttackSkill");
        ParseExcelDataToJson<LocalizationDataLoader, LocalizationData>("Localization");
        ParseExcelDataToJson<PassiveSkillDataLoader, PassiveSkillData>("PassiveSkill");
        ParseExcelDataToJson<CreatureDataLoader, CreatureData>("Creature");
        ParseExcelDataToJson<StageDataLoader, StageData>("Stage");
        ParseExcelDataToJson<WaveDataLoader, WaveData>("Wave");
        ParseExcelDataToJson<AchievementDataLoader, AchievementData>("Achievement");
        ParseExcelDataToJson<EquipmentDataLoader, EquipmentData>("Equipment");
        ParseExcelDataToJson<EquipmentDropDataLoader, EquipmentDropData>("EquipmentDrop");
        ParseExcelDataToJson<DropItemDataLoader, DropItemData>("DropItem");

        ParseExcelDataToJson<InfiniteModeConfigDataLoader, InfiniteModeConfigData>("InfiniteModeConfig");        
        ParseExcelDataToJson<GlobalConfigDataLoader, GlobalConfigData>("GlobalConfig");

        // ParseExcelDataToJson<EquipmentSkillDataLoader, EquipmentSkillData>("EquipmentSkill");
        ParseExcelDataToJson<EquipmentSkillGroupDataLoader, EquipmentSkillGroupData>("EquipmentSkillGroup");
        ParseExcelDataToJson<EvolutionDataLoader, EvolutionData>("Evolution");
        ParseExcelDataToJson<EvolutionOrderLoader, EvolutionOrderData>("EvolutionOrder");
        ParseExcelDataToJson<AccountLevelDataLoader, AccountLevelData>("AccountLevel");
        ParseExcelDataToJson<ItemDataLoader, ItemData>("Item");
        ParseExcelDataToJson<LevelDataLoader, LevelData>("Level");
        ParseExcelDataToJson<QuestDataLoader, QuestData>("Quest");
        ParseExcelDataToJson<DailyQuestRewardDataLoader, DailyQuestRewardData>("DailyQuestReward");
        ParseExcelDataToJson<ShopDataDataLoader, ShopData>("Shop");
        ParseExcelDataToJson<PassRewardDataLoader, PassRewardData>("PassReward");
        ParseExcelDataToJson<ShopSubscribeDataLoader, ShopSubscribeData>("ShopSubscribe");
        ParseExcelDataToJson<HuntPassConfigDataLoader, HuntPassConfigData>("HuntPassConfig");

        // ConvertCsvToJsonBanWords();
        Debug.Log("[DataTransformer] Complete");
    }

    private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
    {
        Loader loader = new Loader();
        FieldInfo field = loader.GetType().GetFields()[0];
        field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        string savePath = Path.Combine(Application.dataPath, "@Resources/Data/JsonData", filename + "Data.json");
        File.WriteAllText(savePath, jsonStr);
        AssetDatabase.Refresh();
    }

    private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
    {
        var path = Path.Combine(Application.dataPath, "@Resources/Data/ExcelData", filename + "Data.csv");
        var lines = File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        string[] headers = lines[0].Replace("\r", "").Split(',');
        Dictionary<string, int> columnIndex = headers
            .Select((name, idx) => new { name = name.Trim(), idx })
            .ToDictionary(x => x.name, x => x.idx);

        var results = new List<LoaderData>();

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Replace("\r", "").Split(',');
            if (row.Length == 0 || string.IsNullOrWhiteSpace(row[0])) continue;

            LoaderData instance = new LoaderData();
            var fields = typeof(LoaderData).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {  
                if (!columnIndex.ContainsKey(field.Name))
                {
                    continue;
                }
                
                int col = columnIndex[field.Name];
                if (col >= row.Length)
                {
                    continue;
                }

                object converted = null;
                string rawValue = row[col];
                
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    converted = ConvertList(rawValue, field.FieldType);
                }
                else
                {
                    converted = ConvertValue(rawValue, field.FieldType);
                }

                if (converted != null)
                {
                    field.SetValue(instance, converted);
                }
            }
            
            results.Add(instance);
        }

        return results;
    }

    private static object ConvertList(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Activator.CreateInstance(type); // 빈 리스트 반환
        }

        Type valueType = type.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(valueType);
        var genericList = Activator.CreateInstance(genericListType) as IList;

        var list = value
            .Split('&')
            .Select(x => ConvertValue(x.Trim(), valueType))
            .Where(x => x != null)
            .ToList();

        foreach (var item in list)
            genericList.Add(item);

        return genericList;
    }

    private static object ConvertValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value)) return null;
        try
        {
            if (type == typeof(string))
            {
                value = value.Replace("\\n", "\n");
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromString(value);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ConvertValue] Failed to convert '{value}' to {type.Name}: {e.Message}");
            return null;
        }
    }
    
    
    public static void ConvertCsvToJsonBanWords()
    {
        // CSV 파일 경로
        var csvPath = Path.Combine(Application.dataPath, "@Resources/Data/ExcelData", "BanWordData.csv");

        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSV 파일이 존재하지 않습니다: {csvPath}");
            return;
        }

        // 모든 줄 읽기
        var lines = File.ReadAllLines(csvPath);
        List<string> badWords = new List<string>();

        foreach (var line in lines)
        {
            var word = line.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                badWords.Add(word);
            }
        }

        // JSON으로 변환
        string json = JsonConvert.SerializeObject(badWords, Formatting.Indented);

        // 저장 경로
        string jsonPath = Path.Combine(Application.dataPath, "@Resources/Data/JsonData", "BanWordData.json");
        File.WriteAllText(jsonPath, json);

        Debug.Log($"BadWords JSON 생성 완료: {jsonPath}");
        AssetDatabase.Refresh();
    }
#endif
}
