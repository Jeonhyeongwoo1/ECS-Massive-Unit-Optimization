using System;
using MewVivor.Enum;
using MewVivor.Interface;
using UniRx;

namespace MewVivor.Model
{
    public class StageModel : IModel
    {
        public readonly ReactiveProperty<int> WaveTimer = new();
        public readonly ReactiveProperty<int> ElapsedGameTime = new();
        public readonly ReactiveProperty<int> CurrentWaveStep = new();
        public readonly ReactiveProperty<int> KillCount = new();

        public readonly ReactiveProperty<int> NormalMonsterKillCount = new();
        public readonly ReactiveProperty<int> EliteMonsterKillCount = new();
        public readonly ReactiveProperty<int> BossMonsterKillCount = new();

        //부활 횟수
        public readonly ReactiveProperty<int> PossibleResurrectionCount = new();

        #region Cheat
        
        public readonly ReactiveProperty<int> StageLevel = new();

        #endregion
        
        
        public void Reset()
        {
            ElapsedGameTime.Value = 0;
            WaveTimer.Value = 0;
            CurrentWaveStep.Value = 0;
            KillCount.Value = 0;
            EliteMonsterKillCount.Value = 0;
            BossMonsterKillCount.Value = 0;
            StageLevel.Value = 0;
        }
        
        public void AddMonsterKillCount(MonsterType monsterType)
        {
            switch (monsterType)
            {
                case MonsterType.Normal:
                    NormalMonsterKillCount.Value++;
                    break;
                case MonsterType.Elite:
                    EliteMonsterKillCount.Value++;
                    break;
                case MonsterType.Boss:
                    BossMonsterKillCount.Value++;
                    break;
            }

            KillCount.Value++;
            Manager.I.GameContinueData.killCount++;
        }
    }
}