#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Presenter;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Serialization;

public partial class GameCheatEditorWindow : OdinEditorWindow
{
    private readonly string OutGameGroup = "OutGameTab";

    private void OnGameStarted()
    {
        var equipmentDataList =Manager.I.Data.EquipmentDataDict.Keys.ToList();
        equipmentList.Clear();
        foreach (int data in equipmentDataList)
        {
            equipmentList.Add(data);
        }
    }

    private void OnDisableOutGameCheat()
    {
        equipmentList.Clear();
    }

    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1001)]
    [ReadOnly]
    public List<int> equipmentList = new();

    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1001)]
    [LabelText("선택한 장비 아이템")]
    public int selectedEquipmentItemValue;

    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1001)]
    [Button("장비추가", ButtonSizes.Medium), GUIColor(1f, 1f, 0f)]
    public async void AddEquipItem()
    {
        CreateUserItemResponseData response =
            await Manager.I.Web.SendRequest<CreateUserItemResponseData>("/sandbox/create-user-equipment",
                new CreateUserItemRequestData()
                {
                    baseItemCode = selectedEquipmentItemValue
                });
    }

    public Dictionary<int, string> ItemNames = new Dictionary<int, string>
    {
        { 50001, "Gold" },
        { 50002, "Jewel" },
        { 50003, "Stamina" },
        { 50004, "Exp" },
        { 50005, "QuestExp" },
        { 50101, "WeaponScroll" },
        { 50102, "GlovesScroll" },
        { 50103, "RingScroll" },
        { 50104, "BeltScroll" },
        { 50105, "ArmorScroll" },
        { 50106, "BootsScroll" },
        { 50201, "ReviveCost" },
        { 50202, "SilverKey" },
        { 50203, "GoldKey" },
        { 50301, "RandomScroll" },
        { 50302, "AllRandomEquipmentBox" },
        { 50303, "CommonEquipmentBox" },
        { 50304, "UncommonEquipmentBox" },
        { 50305, "RareEquipmentBox" },
        { 50306, "EpicEquipmentBox" },
        { 50307, "LegendaryEquipmentBox" },
        { 50401, "InfiniteTicket" }
    };
    
    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1002)]
    [ValueDropdown(nameof(GetItemDropdown))]
    [LabelText("선택한 아이템")]
    public string selectedItemName; 

    private IEnumerable<string> GetItemDropdown()
    {
        var list = ItemNames.Values.ToList();
        return list;
    }

    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1002)]
    [LabelText("아이템 수량")]
    public int selectedItemValue; 

    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1002)]
    [Button("아이템 얻기", ButtonSizes.Medium), GUIColor(1f, 1f, 0f)]
    public void AddItem()
    {
        AddItemAsync();
    }

    private async void AddItemAsync()
    {
        int itemId = 0;
        foreach (var (key, value) in ItemNames)
        {
            if (value == selectedItemName)
            {
                itemId = key;
            }
        }

        Dictionary<string, int> dict = new Dictionary<string, int>();
        dict.Add(itemId.ToString(), selectedItemValue);

        AddItemRequestData itemRequestData = new AddItemRequestData();
        itemRequestData.items = dict;

        var response =
            await Manager.I.Web.SendRequest<AddItemResponseData>("/sandbox/cheat", itemRequestData, MethodType.POST.ToString());
        
        foreach (var (key, value) in response.data)
        {
            Debug.Log($"key {key}/ {value}");
        }   
    }

    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1003)]
    [Button("업적 불러오기", ButtonSizes.Medium), GUIColor(1f, 1f, 0f)]
    public void GetAchievementData()
    {
        var presenter = PresenterFactory.CreateOrGet<QuestPopupPresenter>();
        presenter.GetQuestDataAsync();
    }
    
    [Serializable]
    public class CreateMailData
    {
        public string title;
        public string text;
        public string type;
        [ShowInInspector] public Dictionary<string, int> rewards;
        // public DateTime expiredAt = DateTime.Now;
    }
    
    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1004)]
    [LabelText("메일 데이터")]
    public CreateMailData createMailRequestData;
    
    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1004)]
    [Button("메일 생성", ButtonSizes.Medium)]
    public async void CreateMail()
    {
        var response =
            await Manager.I.Web.SendRequest<GetMailResponseData>("/sandbox/create-mail", createMailRequestData);
        Debug.Log("Response :" + response);
    }
    
    [TabGroup(nameof(OutGameGroup))]
    [PropertyOrder(1004)]
    [Button("유저 데이터 삭제", ButtonSizes.Medium)]
    public async void RemoveUserData()
    {
        PlayerPrefs.DeleteAll();
    }
}

#endif