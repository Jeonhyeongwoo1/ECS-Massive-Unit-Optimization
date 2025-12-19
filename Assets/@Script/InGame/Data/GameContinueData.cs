using System;
using System.Collections.Generic;
using MewVivor.Data;
using MewVivor.InGame.Skill;
using UnityEngine.Serialization;

namespace MewVivor.InGame.Data
{
    [Serializable]
    public class GameContinueData
    {
        public bool IsContinue => killCount > 0;
        
        public int stageIndex;
        public int waveIndex;

        public int killCount;

        public List<AttackSkillData> skillList = new();
        public List<PassiveSkillData> supportSkillDataList = new();
        
        public GameContinueData()
        {
            Reset();
        }

        public void Reset()
        {
            waveIndex = 0;
            stageIndex = 1;
            killCount = 0;
            skillList.Clear();
            supportSkillDataList.Clear();
        }
    }
}