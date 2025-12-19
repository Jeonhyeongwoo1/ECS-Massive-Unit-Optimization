#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MewVivor;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Skill;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public partial class GameCheatEditorWindow : OdinEditorWindow
{
    [System.Serializable]
    public class SkillData
    {
        public string SkillSprite;
        public string PrefabLabel;
        public int SkillCategoryType;
        public int AttackSkillType;
        public float ProjectileSpacing;
        public int NumOfProjectile;
        public float ConeAngle;
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
        public int DebuffType1;
        public float DebuffValuePercent1;
        public float DebuffValue1;
        public float DebuffDuration1;
        public int DebuffType2;
        public float DebuffValuePercent2;
        public float DebuffValue2;
        public float DebuffDuration2;
        public string ApplyType_1;
        public string ApplyType_2;
        public string ApplyType_3;
        public int DataId;
        public string Name;
        public string Description;
        public string IconLabel;
        public int MatchSkillId;
        public int CurrentLevel;
        public int SkillType;
    }

    [System.Serializable]
    public class SkillDataWrapper
    {
        public List<SkillData> skills;
    }
    
    [System.Serializable]
    public class CheatPassiveSkillData
    {
        public int dataId;
        public PassiveSkillType passiveSkillType;
        public string titleTextKey;
        public int currentLevel;
        public float addValue;
    }
    
    private void OnChangedGameScene_Skill(object value)
    {
        OnLearnSkill(Manager.I.Object.Player.SkillBook);
    }
    
#region AttackSkill
    
    private void OnLearnSkill(object value)
    {
        SkillBook skillBook = (SkillBook)value;
        attackSkillInfoDataList.Clear();
        
        foreach (BaseAttackSkill skill in skillBook.ActivateAttackSkillList)
        {
            string title = Manager.I.Data.LocalizationDataDict[skill.AttackSkillData.TitleTextKey].GetValueByLanguage();
            AttackSkillInfoData data = new AttackSkillInfoData();
            data.level = skill.CurrentLevel;
            data.name = title;
            data.AttackSkillData = skill.AttackSkillData;
            attackSkillInfoDataList.Add(data);
        }
        
        OnLearnPassiveSkill(skillBook);
    }
    
    [TabGroup(nameof(SkillGroup))] 
    [PropertyOrder(1)] 
    [LabelText("어택 스킬 ID")] 
    public AttackSkillType attackSkillType;
    
    [PropertyOrder(1)]
    [TabGroup(nameof(SkillGroup))]
    [Button("어택 스킬 얻기 및 업데이트", ButtonSizes.Medium)]
    public void OnLearnAttackSkill()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("에러", "게임 시작해주세요", "예");
            return;
        }

        if (Manager.I.Game.GameState != GameState.Start)
        {
            return;
        }

        Manager.I.Object.Player.Cheat_LearnAttackSkill(attackSkillType);
    }
    
    [PropertyOrder(1)]
    [TabGroup(nameof(SkillGroup))]
    [Button("어택 스킬 없애기", ButtonSizes.Medium)]
    public void OnRemoveAttackSKill()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("에러", "게임 시작해주세요", "예");
            return;
        }

        if (Manager.I.Game.GameState != GameState.Start)
        {
            return;
        }

        bool isSuccessRemoveSkill = Manager.I.Object.Player.Cheat_RemoveAttackSkill(attackSkillType);
        if (isSuccessRemoveSkill)
        {
            attackSkillInfoDataList.Clear();

            SkillBook skillBook = Manager.I.Object.Player.SkillBook;
            foreach (BaseAttackSkill skill in skillBook.ActivateAttackSkillList)
            {
                string title = Manager.I.Data.LocalizationDataDict[skill.AttackSkillData.TitleTextKey].GetValueByLanguage();
                AttackSkillInfoData data = new AttackSkillInfoData();
                data.level = skill.CurrentLevel;
                data.name = title;
                data.AttackSkillData = skill.AttackSkillData;
                attackSkillInfoDataList.Add(data);
            }
        }
    }

    [TabGroup(nameof(SkillGroup))]
    [PropertyOrder(2)]
    [Title("공격 스킬 목록", bold: true, horizontalLine: true)]
    [TableList(AlwaysExpanded = true)]
    public List<AttackSkillInfoData> attackSkillInfoDataList = new();

    [TabGroup(nameof(SkillGroup))]
    [PropertyOrder(3)]
    [Button("변경한 Attack 스킬 데이터 인게임에 적용", ButtonSizes.Large)]
    private void ApplySkill()
    {
        foreach (AttackSkillInfoData attackSkillInfoData in attackSkillInfoDataList)
        {
            AttackSkillData skillData = attackSkillInfoData.AttackSkillData;
            Manager.I.Object.Player.Cheat_UpdateAttackSkillData(skillData);
        }
    }

    private class AttackSkillDataContainer
    {
        public List<AttackSkillData> skills = new();
    }

    private AttackSkillDataContainer _attackSkillDataContainer;
    //C:\Users\guddn\Desktop\workspace\mewvivor_client\Assets\@Resources\Data\JsonData
    private readonly string skillDataPath = $"Assets/@Resources/Data/JsonData/AttackSkillData.json";
    
    public void LoadActiveSkillData()
    {
        //C:\Users\guddn\Desktop\workspace\mewvivor_client\Assets\@Resources\Data\JsonData
        string json = File.ReadAllText(skillDataPath);
        _attackSkillDataContainer = JsonUtility.FromJson<AttackSkillDataContainer>(json);
    }
    
    [TabGroup(nameof(SkillGroup))]
    [PropertyOrder(4)]
    [Button("Attack Skill Json에 변경 내용 적용", ButtonSizes.Medium),GUIColor(1f, 1f, 0f)]
    private void ApplyChangesToOriginal()
    {
        if (_attackSkillDataContainer == null)
        {
            EditorUtility.DisplayDialog("에러", "게임 시작해주세요", "예");
            return;
        }

        var list = Manager.I.Object.Player.SkillBook.ActivateAttackSkillList;
        foreach (BaseAttackSkill skill in list)
        {
            var original = _attackSkillDataContainer.skills.FirstOrDefault(x => x.DataId == skill.AttackSkillData.DataId);
            if (original != null)
            {
                string jsonData = JsonUtility.ToJson(skill.AttackSkillData);
                Newtonsoft.Json.JsonConvert.PopulateObject(jsonData, original);
            }
        }

        string json = JsonUtility.ToJson(_attackSkillDataContainer, true);
        File.WriteAllText(skillDataPath, json);
        AssetDatabase.Refresh();
    }

    [TabGroup(nameof(SkillGroup))]
    [PropertyOrder(5)]
    [Button("Attack Skill Json -> CSv 변경", ButtonSizes.Medium), GUIColor(1f, 0f, 0f)]
    public static void ConvertJsonToCsv()
    {
        string jsonPath = Application.dataPath + "/@Resources/Data/JsonData/AttackSkillData.json";
        if (string.IsNullOrEmpty(jsonPath)) return;

        string json = File.ReadAllText(jsonPath);
        var dataWrapper = JsonUtility.FromJson<SkillDataWrapper>(json);
        if (dataWrapper == null || dataWrapper.skills == null)
        {
            Debug.LogError("JSON 파싱 실패 또는 'skills' 필드 없음");
            return;
        }

        // enum 매핑
        Dictionary<int, string> debuffTypeMap = new()
        {
            { 0, "None" }, { 1, "Stun" }, { 2, "AddDamage" }, { 3, "SlowSpeed" }, { 4, "DotDamage" }
        };
        Dictionary<int, string> skillTypeMap = new()
        {
            { 0, "None" }, { 1, "AttackSkill" }, { 2, "PassiveSkill" }
        };
        Dictionary<int, string> attackSkillTypeMap = new()
        {
            { 0, "None" },
            { 10001, "Skill_10001" }, { 10011, "Skill_10011" }, { 10021, "Skill_10021" },
            { 10031, "Skill_10031" }, { 10041, "Skill_10041" }, { 10051, "Skill_10051" },
            { 10061, "Skill_10061" }, { 10071, "Skill_10071" }, { 10081, "Skill_10081" },
            { 10091, "Skill_10091" }, { 10101, "Skill_10101" }, { 10111, "Skill_10111" },
            { 10121, "Skill_10121" }, { 10131, "Skill_10131" }, { 10141, "Skill_10141" }
        };
        Dictionary<int, string> skillCategoryTypeMap = new()
        {
            { 0, "Normal" }, { 1, "Ultimate" }
        };

        var sb = new StringBuilder();
        sb.AppendLine(
            "DataId,TitleTextKey,DescriptionTextKey,IconLabel,SkillSprite,PrefabLabel,SkillCategoryType,AttackSkillType,ProjectileSpacing,NumOfProjectile,ConeAngle,AttackRange,Scale,DamagePercent,CoolTime,KnockbackDistance,ProjectileSpeed,Efficiency,EfficiencyPercent,SkillDuration,AttackInterval,DebuffType1,DebuffValuePercent1,DebuffValue1,DebuffDuration1,DebuffType2,DebuffValuePercent2,DebuffValue2,DebuffDuration2,MatchSkillId,SkillType");

        foreach (var skill in dataWrapper.skills)
        {
            string line = $"{skill.DataId}," +
                          $"SkillTitle_{skill.DataId}," +
                          $"SkillDescription_{skill.DataId}," +
                          $"{skill.IconLabel}," +
                          $"{skill.SkillSprite}," +
                          $"{skill.PrefabLabel}," +
                          $"{(skillCategoryTypeMap.TryGetValue(skill.SkillCategoryType, out var cat) ? cat : skill.SkillCategoryType.ToString())}," +
                          $"{(attackSkillTypeMap.TryGetValue(skill.AttackSkillType, out var atk) ? atk : skill.AttackSkillType.ToString())}," +
                          $"{skill.ProjectileSpacing}," +
                          $"{skill.NumOfProjectile}," +
                          $"{skill.ConeAngle}," +
                          $"{skill.AttackRange}," +
                          $"{skill.Scale}," +
                          $"{skill.DamagePercent}," +
                          $"{skill.CoolTime}," +
                          $"{skill.KnockbackDistance}," +
                          $"{skill.ProjectileSpeed}," +
                          $"{skill.Efficiency}," +
                          $"{skill.EfficiencyPercent}," +
                          $"{skill.SkillDuration}," +
                          $"{skill.AttackInterval}," +
                          $"{(debuffTypeMap.TryGetValue(skill.DebuffType1, out var d1) ? d1 : skill.DebuffType1.ToString())}," +
                          $"{skill.DebuffValuePercent1}," +
                          $"{skill.DebuffValue1}," +
                          $"{skill.DebuffDuration1}," +
                          $"{(debuffTypeMap.TryGetValue(skill.DebuffType2, out var d2) ? d2 : skill.DebuffType2.ToString())}," +
                          $"{skill.DebuffValuePercent2}," +
                          $"{skill.DebuffValue2}," +
                          $"{skill.DebuffDuration2}," +
                          $"{skill.MatchSkillId}," +
                          $"{(skillTypeMap.TryGetValue(skill.SkillType, out var st) ? st : skill.SkillType.ToString())}";

            sb.AppendLine(line);
        }

        // string folderPath = Path.Combine(Application.dataPath, "Exported");
        // Directory.CreateDirectory(folderPath);

        string csvPath = Application.dataPath + "/@Resources/Data/ExcelData/AttackSkillData.csv";
        File.WriteAllText(csvPath, sb.ToString(), Encoding.UTF8);

        Debug.Log($"CSV 저장 완료: {jsonPath}");
        AssetDatabase.Refresh();
    }
    
#endregion
}
#endif