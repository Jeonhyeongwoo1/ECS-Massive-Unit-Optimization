#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Skill;
using MewVivor.InGame.Stage;
using MewVivor.Model;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public partial class GameCheatEditorWindow : OdinEditorWindow
{
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private void OnChangedGameScene_GameSystem(object value)
    {
        CalculateMonsterAtkAndHp();
        CalculatePlayerStat();
    }

    private void OnDisableGameSystem()
    {
        cheatPassiveSkillDataList?.Clear();
        normalMonsterTotalHPList?.Clear();
        normalMonsterTotalAtkList?.Clear();
        monsterID = 0;
        CurrentPlayerExp = 0;
        CurrentPlayerLevel = 0;
        _isPlayerInvincibility = false;
        EquippedItemDict?.Clear();
        if (_disposables != null)
        {
            _disposables.Dispose();
        }

        CurrentPlayerLevel = 0;
        CurrentPlayerExp = 0;
        PlayerMaxHPValue = 0;
        PlayerAtkValue = 0;
        PlayerMoveSpeedValue = 0;
        PlayerCriticalPercentValue = 0;
        PlayerHpRecoveryValue = 0;
        PlayerCriticalDamagePercentValue = 0;
        CurrentPlayerHP = 0;
        currentWaveLevel = 0;
        currentStageLevel = 0;
        PlayerDefenseValue = 0;
        PlayerExpValue = 0;
        PlayerAddGoldAmountValue = 0;
        PlayerMagneticRangePercentValue = 0;
        PlayerSkillCoolTimeValue = 0;
        PlayerHpRecoveryEfficiencyValue = 0;
        PlayerAuraDamageUpPercentValue = 0;
        PlayerExplosionSkillSizeValue = 0;
        PlayerBulletSkillSizeValue = 0;
        PlayerCircleSkillSizeValue = 0;
        PlayerItemBoxSpawnCoolTimeValue = 0;
        PlayerReviveCountBySkill = 0;
    }

    private void CalculatePlayerStat()
    {
        #region Subscirbe
        StageModel stageModel = ModelFactory.CreateOrGetModel<StageModel>();
        GameManager gameManager = Manager.I.Game;
        PlayerModel playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
        playerModel.CurrentLevel.Subscribe(x =>
        {
            CurrentPlayerLevel = x;
            Repaint();
        }).AddTo(_disposables);
        playerModel.CurrentExp.Subscribe(x =>
        {
            CurrentPlayerExp = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.MaxHP.Subscribe(x =>
        {
            PlayerMaxHPValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.Atk.Subscribe(x =>
        {
            PlayerAtkValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.MoveSpeed.Subscribe(x =>
        {
            PlayerMoveSpeedValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.CriticalPercent.Subscribe(x =>
        {
            PlayerCriticalPercentValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.HpRecovery.Subscribe(x =>
        {
            PlayerHpRecoveryValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.CriticalDamagePercent.Subscribe(x =>
        {
            PlayerCriticalDamagePercentValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.CurrentHP.Subscribe(x =>
        {
            CurrentPlayerHP = x;
            Repaint();
        }).AddTo(_disposables);

        stageModel.CurrentWaveStep.Subscribe(x =>
        {
            currentWaveLevel = x;
            Repaint();
        }).AddTo(_disposables);

        stageModel.StageLevel.Subscribe(x =>
        {
            currentStageLevel = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.Defense.Subscribe(x =>
        {
            PlayerDefenseValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.Exp.Subscribe(x =>
        {
            PlayerExpValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.AddGoldAmount.Subscribe(x =>
        {
            PlayerAddGoldAmountValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.MagneticRangePercent.Subscribe(x =>
        {
            PlayerMagneticRangePercentValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.SkillCoolTime.Subscribe(x =>
        {
            PlayerSkillCoolTimeValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.HpRecoveryEfficiency.Subscribe(x =>
        {
            PlayerHpRecoveryEfficiencyValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.AuraDamageUpPercent.Subscribe(x =>
        {
            PlayerAuraDamageUpPercentValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.ExplosionSkillSize.Subscribe(x =>
        {
            PlayerExplosionSkillSizeValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.BulletSkillSize.Subscribe(x =>
        {
            PlayerBulletSkillSizeValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.CircleSkillSize.Subscribe(x =>
        {
            PlayerCircleSkillSizeValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.ItemBoxSpawnCoolTime.Subscribe(x =>
        {
            PlayerItemBoxSpawnCoolTimeValue = x;
            Repaint();
        }).AddTo(_disposables);

        playerModel.ReviveCountBySkill.Subscribe(x =>
        {
            PlayerReviveCountBySkill = x;
            Repaint();
        }).AddTo(_disposables);

        UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
        var list = userModel.GetEquippedItemList();
        EquippedItemDict = new Dictionary<EquipmentType, EquippedSkillData>();
        
        foreach (Equipment equipment in list)
        {
            EquippedSkillData data = new EquippedSkillData();
            data.EquippedItemCode = equipment.EquipmentData.ItemCode;
            Dictionary<EquipmentGrade, int> skillDict = new();
            foreach (var (key, value) in equipment.EquipmentSkillDataDict)
            {
                skillDict[key] = value.DataId;
            }

            data.skillDict = skillDict;
            EquippedItemDict[equipment.EquipmentType] = data;
        }

        #endregion
    }

    private void OnLearnPassiveSkill(SkillBook skillBook)
    {
        cheatPassiveSkillDataList.Clear();
        foreach (BasePassiveSkill basePassiveSkill in skillBook.ActivatePassiveSkillList)
        {
            CheatPassiveSkillData data = new CheatPassiveSkillData();
            data.dataId = basePassiveSkill.PassiveSkillData.DataId;
            data.passiveSkillType = basePassiveSkill.PassiveSkillType;
            data.titleTextKey = basePassiveSkill.PassiveSkillData.TitleTextKey;
            data.currentLevel = basePassiveSkill.CurrentLevel;
            data.addValue = basePassiveSkill.PassiveSkillData.Rate * basePassiveSkill.CurrentLevel;
            
            cheatPassiveSkillDataList.Add(data);
        }
    }

    private void CalculateMonsterAtkAndHp()
    {
        GameManager gameManager = Manager.I.Game;
        StageModel stageModel = ModelFactory.CreateOrGetModel<StageModel>();
        stageModel.CurrentWaveStep.Subscribe(x =>
        {
            if (gameManager.GameType == GameType.MAIN)
            {
                NormalStage normalStage = gameManager.CurrentStage as NormalStage;
                WaveData waveData = normalStage.CurrentWaveData;
                List<int> monsterIds = waveData.MonsterId;
                normalMonsterTotalAtkList.Clear();
                normalMonsterTotalHPList.Clear();
                monsterIds.ForEach(v =>
                {
                    CreatureData creatureData = Manager.I.Data.CreatureDict[v];
                    (float atk, float hp) = gameManager.GetMonsterAtkAndHP(MonsterType.Normal, creatureData);
                    normalMonsterTotalAtkList.Add((int)atk);
                    normalMonsterTotalHPList.Add((int)hp);
                });
            }
            else if (gameManager.GameType == GameType.INFINITY)
            {
                (float finalAtk, float finalHp) = gameManager.GetMonsterAtkAndHP(MonsterType.Normal, null);
                normalMonsterTotalAtkList.Add((int)finalAtk);
                normalMonsterTotalHPList.Add((int)finalHp);
            }
        });
    }
    
    [Title("게임")]
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [LabelText("스테이지 선택")]
    [OnValueChanged(nameof(OnChangeStage))]
    public int stageIndex = 1;
    
    private void OnChangeStage()
    {
        Manager.I.SelectedStageIndex = stageIndex;
        Debug.Log($"{stageIndex} / {Manager.I.SelectedStageIndex}");
    }

    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [LabelText("스테이지 선택")]
    public bool isSelectStage;
    
    
    #region Monster
    [TabGroup("GameSystemTab")]
    [PropertyOrder(1)] [OnValueChanged(nameof(SpawnMonster))]
    [LabelText("몬스터 소환 여부")]
    public bool isSpawnMonster = true;
    private void SpawnMonster()
    {
        Manager.I.Object.isSpawnMonster = isSpawnMonster;
    }

    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [Title("몬스터")]
    [LabelText("몬스터 아이디")]
    public int monsterID;

    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [LabelText("몬스터 스폰 갯수")]
    public int monsterSpawnCount;
     
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [LabelText("몬스터 크기")]
    public float monsterScale;

    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [Button("특정 몬스터 소환", ButtonSizes.Medium)]
    public void OnCreateBossMonster()
    {
        Manager.I.Game.Cheat_SpawnMonster(monsterID, monsterSpawnCount, monsterScale);
    }
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(1)]
    [Button("모든 몬스터 죽이기", ButtonSizes.Medium)]
    public void OnKillMonster()
    {
        foreach (MonsterController monsterController in Manager.I.Object.ActivateMonsterList)
        {
            monsterController.ForceKill();
        }
    }
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(2)]
    [Sirenix.OdinInspector.ReadOnly, LabelText("일반 몬스터 총 공격력")]
    public List<int> normalMonsterTotalAtkList;
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(2)]
    [Sirenix.OdinInspector.ReadOnly, LabelText("일반 몬스터 총 HP")]
    public List<int> normalMonsterTotalHPList;

    #endregion    
    
    #region Character

    [TabGroup("GameSystemTab")]
    [Title("Player")]
    [Button("캐릭터 죽음", ButtonSizes.Medium)]
    [PropertyOrder(100)]
    public void OnDeadPlayer()
    {
        PlayerController player = Manager.I.Object.Player;
        int damage = (int)player.HP.Value * 2;
        player.TakeDamage(damage, null);
    }
    
    private bool _isPlayerInvincibility;

    [TabGroup("GameSystemTab")]
    [ShowIf("@!_isPlayerInvincibility")] // 무적이 아닐 때만 보여줌
    [Button("캐릭터 무적", ButtonSizes.Medium)]
    [PropertyOrder(100)]
    public void OnUpdatePlayerInvincibilityState()
    {
        PlayerController player = Manager.I.Object.Player;
        player.UpdatePlayerState(PlayerStateType.Invincibility);
        _isPlayerInvincibility = true;
    }
    
    [TabGroup("GameSystemTab")]
    [ShowIf(nameof(_isPlayerInvincibility))] // 무적일 때만 보여줌
    [Button("캐릭터 무적 비활성화", ButtonSizes.Medium)]
    [PropertyOrder(100)]
    public void OnUpdatePlayerNormalState()
    {
        PlayerController player = Manager.I.Object.Player;
        player.UpdatePlayerState(PlayerStateType.Normal);
        _isPlayerInvincibility = false;
    }
    
    [TabGroup("GameSystemTab")]
    [Button("캐릭터 레벨업", ButtonSizes.Medium)]
    [PropertyOrder(100)]
    public void OnLevelUp()
    {
        PlayerController player = Manager.I.Object.Player;
        PlayerModel playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
        var levelData = Manager.I.Data.LevelDataDict[playerModel.CurrentLevel.Value];
        float remainExp = levelData.AccumulatedEXP - player.CurrentExp;
        player.CurrentExp = remainExp;
    }
    
    [TabGroup("GameSystemTab")]
    [LabelText("플레이어 크기 값")]
    [PropertyOrder(100)]
    public int playerScale;
    [TabGroup("GameSystemTab")]
    [Button("캐릭터 크기 조절", ButtonSizes.Medium)]
    [PropertyOrder(100)]
    public void OnChangePlayerScale()
    {
        PlayerController player = Manager.I.Object.Player;
        player.Cheat_ResizeScale(playerScale);
    }
    

    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 경험치")]
    [PropertyOrder(101)]
    public int CurrentPlayerExp;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 레벨")]
    [PropertyOrder(101)]
    public int CurrentPlayerLevel;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 현재 체력")]
    [PropertyOrder(101)]
    public float CurrentPlayerHP;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 MaxHP")]
    [PropertyOrder(102)]
    public float PlayerMaxHPValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 Atk")]
    [PropertyOrder(102)]
    public float PlayerAtkValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 MoveSpeed")]
    [PropertyOrder(102)]
    public float PlayerMoveSpeedValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 HpRecovery")]
    [PropertyOrder(102)]
    public float PlayerHpRecoveryValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 CriticalPercent")]
    [PropertyOrder(102)]
    public float PlayerCriticalPercentValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 CriticalDamagePercent")]
    [PropertyOrder(102)]
    public float PlayerCriticalDamagePercentValue;

    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 Defense")] 
    [PropertyOrder(102)]
    public float PlayerDefenseValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 Exp")] 
    [PropertyOrder(102)]
    public float PlayerExpValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 AddGoldAmount")] 
    [PropertyOrder(102)]
    public float PlayerAddGoldAmountValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 MagneticRangePercent")] 
    [PropertyOrder(102)]
    public float PlayerMagneticRangePercentValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 SkillCoolTime")] 
    [PropertyOrder(102)]
    public float PlayerSkillCoolTimeValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 HpRecoveryEfficiency")] 
    [PropertyOrder(102)]
    public float PlayerHpRecoveryEfficiencyValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 AuraDamageUpPercent")] 
    [PropertyOrder(102)]
    public float PlayerAuraDamageUpPercentValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 ExplosionSkillSize")] 
    [PropertyOrder(102)]
    public float PlayerExplosionSkillSizeValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 BulletSkillSize")] 
    [PropertyOrder(102)]
    public float PlayerBulletSkillSizeValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 CircleSkillSize")] 
    [PropertyOrder(102)]
    public float PlayerCircleSkillSizeValue;
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 ItemBoxSpawnCoolTime")] 
    [PropertyOrder(102)]
    public float PlayerItemBoxSpawnCoolTimeValue;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 부활 횟수 (skill)")] 
    [PropertyOrder(102)]
    public float PlayerReviveCountBySkill;

    
    [TabGroup("GameSystemTab")] 
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 장비 리스트")] 
    [PropertyOrder(102)]
    public Dictionary<EquipmentType, EquippedSkillData> EquippedItemDict;
    
    [TabGroup("GameSystemTab")]
    [Sirenix.OdinInspector.ReadOnly, LabelText("플레이어 패시브 스킬")]
    [PropertyOrder(102)]
    public List<CheatPassiveSkillData> cheatPassiveSkillDataList;
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(103)] 
    [LabelText("패시브 스킬 ID")] 
    public PassiveSkillType passiveSkillType;

    [PropertyOrder(103)]
    [TabGroup("GameSystemTab")]
    [Button("패시브 스킬 얻기 및 업데이트", ButtonSizes.Medium)]
    public void OnLearnPassiveSkill()
    {
        if (Manager.I.Game.GameState != GameState.Start)
        {
            return;
        }

        Manager.I.Object.Player.Cheat_LearnPassiveSkill(passiveSkillType);
    }
    #endregion

    #region DropItem

    public Dictionary<int, string> dropItemIdDict = new Dictionary<int, string>()
    {
        { 60001, "아이템박스 - 랜덤" },
        { 60002, "아이템 박스 - 자석" },
        { 60003, "아이템박스 - 소형 폭탄" },
        { 60004, "아이템 박스 - 폭탄" },
        { 60005, "아이템박스 - 소형 구급상자" },
        { 60006, "아이템 박스 - 중형 구급상자" },
        { 60007, "아이템 박스 - 스킬업 아이템" },
        { 60008, "아이템박스 - 10골드" },
        { 60009, "아이템 박스 - 50골드" },
        { 60010, "아이템박스 - 100 골드" },
        { 60011, "아이템 박스 -300 골드" },
        { 60012, "아이템박스 - 보석1개" },
        { 60013, "아이템 박스 -보석5개" },
        { 60014, "아이템박스 -보석10개" },
    };
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(200)]
    [Title("드랍 아이템")] 
    [ValueDropdown(nameof(GetDropItemDropdown))]
    [LabelText("선택한 드랍 아이템")]
    public string selectedDropItemValue;
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(201)]
    [Button("드랍 아이템 소환", ButtonSizes.Medium)]
    public void OnCreateDropItem()
    {
        int id = 0;
        foreach (var (key, value) in dropItemIdDict)
        {
            if (value == selectedDropItemValue)
            {
                id = key;
            }
        }
        
        var dropItemDataDict = Manager.I.Data.DropItemDict[id];
        Vector3 spawnPosition =
            Manager.I.Object.Player.Position + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10));
        Manager.I.Game.SpawnDropItem(dropItemDataDict, spawnPosition);
    }
    
    private IEnumerable<string> GetDropItemDropdown()
    {
        var list = dropItemIdDict.Values.ToList();
        return list;
    }

    #endregion

    #region Stage
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(300)] 
    [Title("스테이지")]
    [ReadOnly, LabelText("현재 스테이지 단계")]
    public int currentStageLevel;
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(301)]
    [ReadOnly, LabelText("현재 웨이브 단계")]
    public int currentWaveLevel;

    [TabGroup("GameSystemTab")] 
    [PropertyOrder(301)]
    [Button("다음 웨이브 단계로 넘어가기(보상 x)", ButtonSizes.Medium)]
    public void OnChangeNextWave()
    {
        StageBase stage = Manager.I.Game.CurrentStage;
        NormalStage normalStage = stage as NormalStage;
        normalStage.ForceShutDownWave();
    }
    
    [TabGroup("GameSystemTab")] 
    [PropertyOrder(302)]
    [Button("스테이지 종료 (승리)", ButtonSizes.Medium)]
    public void OnWinStage()
    {
        var normalStage = Manager.I.Game.CurrentStage as NormalStage;
        normalStage.Cheat_CompletedStage(true);
    }

    [TabGroup("GameSystemTab")] 
    [PropertyOrder(302)]
    [Button("스테이지 종료 (패배)", ButtonSizes.Medium)]
    public void OnDefeatStage()
    {
        var normalStage = Manager.I.Game.CurrentStage as NormalStage;
        normalStage.Cheat_CompletedStage(false);
    }

    #endregion
}

[Serializable]
public struct EquippedSkillData
{
    public int EquippedItemCode;
    public Dictionary<EquipmentGrade, int> skillDict;
}
#endif