using MewVivor;
using UnityEngine;
using Sirenix.OdinInspector;
using MewVivor.Enum;
using MewVivor.Managers;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CheatSupportSkillConfigData", order = 1)]
public class CheatSupportSkillConfigData : ScriptableObject
{
    // [OnValueChanged(nameof(OnSoulAmountChanged))]
    // public int soulAmount;

    // [FormerlySerializedAs("supportSkillName")] public PassiveSkillName passiveSkillName;
    // public SupportSkillGrade supportSkillGrade;
    //
    // [Button]
    // public void AddSupportSkill()
    // {
    //     foreach (var (key, value) in Manager.I.Data.SupportSkillDataDict)
    //     {
    //         if (value.SupportSkillName == passiveSkillName && value.SupportSkillGrade == supportSkillGrade)
    //         {
    //             if (value.IsPurchased)
    //             {
    //                 return;
    //             }
    //             
    //             bool isSuccess = Manager.I.Object.Player.TryPurchaseSupportSkill(key);
    //             if (!isSuccess)
    //             {
    //                 Debug.LogError($"Failed purchase support skill {key} / name {passiveSkillName}");
    //             }
    //
    //             break;
    //         }
    //     }
    // }
    //
    // public void OnSoulAmountChanged(int soulAmount)
    // {
    //     Manager.I.Object.Player.SoulAmount = soulAmount;
    // }
}
