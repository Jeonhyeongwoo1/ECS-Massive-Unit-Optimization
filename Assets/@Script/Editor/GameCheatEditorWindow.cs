#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MewVivor;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Skill;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameCheatEditorWindow : OdinEditorWindow
{
    private readonly string SkillGroup = "SkillTab";
    
    [Serializable]
    public class SKillInfoData
    {
        public int skillId;
        public string skillName;
        public AttackSkillType SkillType;
    }
    
    [Serializable]
    public class AttackSkillInfoData
    {
        [TableColumnWidth(50, Resizable = false)]
        public string name;
        [TableColumnWidth(50, Resizable = false)]
        public int level;
        [TableColumnWidth(100, Resizable = true)]
        public AttackSkillData AttackSkillData;
    }
    
    [MenuItem("Tools/GameCheatEditorWindow")]
    private static void OpenWindow()
    {
        var window = GetWindow<GameCheatEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        window.Show();
    }

    protected override void OnEnable()
    {
        Debug.Log("OnEnable");
        base.OnEnable();
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        _isPlayerInvincibility = false;
        OnDisableGameSystem();
        OnDisableOutGameCheat();
    }
    
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingPlayMode:
                Debug.Log("🟥 게임 종료됨 (Play Mode → Edit Mode)");
                attackSkillInfoDataList.Clear();
                _isPlayerInvincibility = false;
                break;

            case PlayModeStateChange.EnteredPlayMode:
                Debug.Log("▶ 게임 시작됨 (Edit Mode → Play Mode)");
                attackSkillInfoDataList.Clear();
                break;
        }

        attackSkillType = AttackSkillType.None;
        OnGameStarted();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLoadGameScene(scene.name);
        OnLoadLobbyScene(scene.name);
    }

    private void OnLoadLobbyScene(string sceneName)
    {
        if (sceneName!= SceneType.LobbyScene.ToString())
        {
            return;
        }
        
        if(!isSelectStage)
        {
            return;
        }
        
        Manager.I.SelectedStageIndex = stageIndex;
        Debug.Log($"{stageIndex} / {Manager.I.SelectedStageIndex}");
    }

    private void OnLoadGameScene(string sceneName)
    {
        if (sceneName!= SceneType.GameScene.ToString())
        {
            return;
        }
        
        // Manager.I.Event.AddEvent(GameEventType.LearnSkill, OnLearnSkill);
        // Manager.I.Event.AddEvent(GameEventType.GameStart, OnGameChanged);
        LoadActiveSkillData();
    }

    private void OnGameChanged(object value)
    {
        OnChangedGameScene_GameSystem(value);
        OnChangedGameScene_Skill(value);
    }
}
#endif