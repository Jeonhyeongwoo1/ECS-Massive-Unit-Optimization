using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Enum;
using MewVivor.Interface;
using UniRx;
using UnityEngine;

namespace MewVivor.Model
{
    //인게임 내에서만 사용하는 Player 모델, 게임 시작과 게임 종료 시 모든 데이터는 초기화
    public class PlayerModel : IModel
    {
        public string GameSessionId { get; private set; }
        public ReactiveProperty<float> CurrentExpRatio = new();
        public ReactiveProperty<int> CurrentLevel = new();
        public ReactiveProperty<int> CurrentExp = new();
        public ReactiveProperty<int> Gold = new();
        public ReactiveProperty<int> Jewel = new();
        public ReactiveProperty<int> CurrentSkillSelectCount = new();
        public ReactiveDictionary<int, int> AcquiredRewardItemDict = new();
        public ReactiveProperty<int> SkillSelectRefreshUseCount = new();
        public ReactiveProperty<int> ResurrectionUseCount = new();
        public ReactiveProperty<int> ReviveCountBySkill = new ReactiveProperty<int>();
        
        #region Cheat_CreatureStat

        public ReactiveProperty<float> MaxHP = new ReactiveProperty<float>();
        public ReactiveProperty<float> Atk = new ReactiveProperty<float>();
        public ReactiveProperty<float> MoveSpeed = new();
        public ReactiveProperty<float> CriticalPercent = new ReactiveProperty<float>();
        public ReactiveProperty<float> CriticalDamagePercent = new ReactiveProperty<float>();
        public ReactiveProperty<float> HpRecovery = new ReactiveProperty<float>();
        public ReactiveProperty<float> CurrentHP = new ReactiveProperty<float>();
        public ReactiveProperty<float> Defense = new ReactiveProperty<float>();
        public ReactiveProperty<float> Exp = new ReactiveProperty<float>();
        public ReactiveProperty<float> AddGoldAmount = new ReactiveProperty<float>();
        public ReactiveProperty<float> MagneticRangePercent = new ReactiveProperty<float>();
        public ReactiveProperty<float> SkillCoolTime = new ReactiveProperty<float>();
        public ReactiveProperty<float> HpRecoveryEfficiency = new ReactiveProperty<float>();
        public ReactiveProperty<float> AuraDamageUpPercent = new ReactiveProperty<float>();
        public ReactiveProperty<float> ExplosionSkillSize = new ReactiveProperty<float>();
        public ReactiveProperty<float> BulletSkillSize = new ReactiveProperty<float>();
        public ReactiveProperty<float> CircleSkillSize = new();
        public ReactiveProperty<float> ItemBoxSpawnCoolTime = new ReactiveProperty<float>();
        
        public void UpdatePlayerStateData(CreatureStatType creatureStatType, float value)
        {
            switch (creatureStatType)
            {
                case CreatureStatType.MaxHP:
                    MaxHP.Value = value;
                    break;
                case CreatureStatType.Atk:
                    Atk.Value = value;
                    break;
                case CreatureStatType.MoveSpeed:
                    MoveSpeed.Value = value;
                    break;
                case CreatureStatType.CriticalPercent:
                    CriticalPercent.Value = value;
                    break;
                case CreatureStatType.HpRecovery:
                    HpRecovery.Value = value;
                    break;
                case CreatureStatType.CriticalDamagePercent:
                    CriticalDamagePercent.Value = value;
                    break;
                case CreatureStatType.Defense:
                    Defense.Value = value;
                    break;
                case CreatureStatType.Exp:
                    Exp.Value = value;
                    break;
                case CreatureStatType.AddGoldAmount:
                    AddGoldAmount.Value = value;
                    break;
                case CreatureStatType.MagneticRangePercent:
                    MagneticRangePercent.Value = value;
                    break;
                case CreatureStatType.SkillCoolTime:
                    SkillCoolTime.Value = value;
                    break;
                case CreatureStatType.HpRecoveryEfficiency:
                    HpRecoveryEfficiency.Value = value;
                    break;
                case CreatureStatType.AuraDamageUpPercent:
                    AuraDamageUpPercent.Value = value;
                    break;
                case CreatureStatType.ExplosionSkillSize:
                    ExplosionSkillSize.Value = value;
                    break;
                case CreatureStatType.BulletSkillSize:
                    BulletSkillSize.Value = value;
                    break;
                case CreatureStatType.CircleSkillSize:
                    CircleSkillSize.Value = value;
                    break;
                case CreatureStatType.ItemBoxSpawnCoolTime:
                    ItemBoxSpawnCoolTime.Value = value;
                    break;
            }
        }
        #endregion

        public void SetGameSessionId(string id)
        {
            GameSessionId = id;
        }
        
        public Dictionary<int, int> GetAcquiredRewardItemDict()
        {
            return AcquiredRewardItemDict.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        
        public void Reset()
        {
            GameSessionId = null;
            CurrentExp.Value = 0;
            CurrentLevel.Value = 1;
            CurrentExpRatio.Value = 0;
            Gold.Value = 0;
            Jewel.Value = 0;
            CurrentSkillSelectCount.Value = 0;
            AcquiredRewardItemDict.Clear();
            SkillSelectRefreshUseCount.Value = 0;
            ResurrectionUseCount.Value = 0;
            ReviveCountBySkill.Value = 0;

            MaxHP.Value = 0;
            Atk.Value = 0;
            MoveSpeed.Value = 0;
            CriticalPercent.Value = 0;
            CriticalDamagePercent.Value = 0;
            HpRecovery.Value = 0;
            CurrentHP.Value = 0;
            Defense.Value = 0;
            Exp.Value = 0;
            AddGoldAmount.Value = 0;
            MagneticRangePercent.Value = 0;
            SkillCoolTime.Value = 0;
            HpRecoveryEfficiency.Value = 0;
            AuraDamageUpPercent.Value = 0;
            ExplosionSkillSize.Value = 0;
            BulletSkillSize.Value = 0;
            CircleSkillSize.Value = 0;
            ItemBoxSpawnCoolTime.Value = 0;
        }
    }
}