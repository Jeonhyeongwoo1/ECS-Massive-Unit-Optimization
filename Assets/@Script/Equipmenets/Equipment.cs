using System.Collections.Generic;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Managers;
using MewVivor.Model;
using UnityEngine;
using EquipmentLevelData = MewVivor.Data.EquipmentLevelData;

namespace MewVivor.Equipmenets
{
    
    public class Equipment
    {
        public int Level => _userEquipmentData.level;
        public EquipmentData EquipmentData => _equipmentData;
        public string UID => _userEquipmentData.userEquipmentId;
        public int EquipmentBaseItemId => _userEquipmentData.baseItemId;
        public EquipmentType EquipmentType => _equipmentData.EquipmentType;
        public EquipmentGrade Grade => _equipmentData.Grade;
        public bool IsMaxLevel => Level == _equipmentData.Grade_MaxLevel;
        public IReadOnlyDictionary<EquipmentGrade, EquipmentSkillData> EquipmentSkillDataDict =>
            _equipmentSkillDataDict;
        
        //Sheet Data
        private EquipmentData _equipmentData;
        private EquipmentSkillGroupData _equipmentSkillGroupData;
        private Dictionary<EquipmentGrade, EquipmentSkillData> _equipmentSkillDataDict = new();
        
        //Server Data
        private UserEquipmentData _userEquipmentData;

        public Equipment(EquipmentData equipmentData, UserEquipmentData userEquipmentData)
        {
            _equipmentData = equipmentData;
            SetUserEquipmentData(userEquipmentData);
        }

        public void SetUserEquipmentData(UserEquipmentData userEquipmentData)
        {
            _userEquipmentData = userEquipmentData;
            
            //UserData가 변경되면 장비의 Sheet Data도 같이 변경해준다.
            int equipmentId = _userEquipmentData.baseItemId;
            DataManager dataManager = Manager.I.Data;
            EquipmentData equipmentData = dataManager.EquipmentDataDict[equipmentId];
            _equipmentData = equipmentData;

            int skillId = equipmentData.SkillGroupDataId;
            EquipmentSkillGroupData skillGroupData = dataManager.EquipmentSkillGroupDataDict[skillId];
            _equipmentSkillGroupData = skillGroupData;
            AddEquipmentSKill(_equipmentData.Grade);
        }

        private void AddEquipmentSKill(EquipmentGrade equipmentGrade)
        {
            _equipmentSkillDataDict.Clear();
            DataManager dataManager = Manager.I.Data;
            int count = System.Enum.GetValues(typeof(EquipmentGrade)).Length;
            for (int i = 0; i < count; i++)
            {
                EquipmentGrade target = (EquipmentGrade)i;
                if (target <= equipmentGrade)
                {
                    switch (target)
                    {
                        case EquipmentGrade.Uncommon:
                            EquipmentSkillData unCommonSkillData =
                                dataManager.EquipmentSkillDataDict[_equipmentSkillGroupData.UncommonGradeSkill];
                            _equipmentSkillDataDict[EquipmentGrade.Uncommon] = unCommonSkillData;
                            break;
                        case EquipmentGrade.Rare:
                            EquipmentSkillData rareData =
                                dataManager.EquipmentSkillDataDict[_equipmentSkillGroupData.RareGradeSkill];
                            _equipmentSkillDataDict[EquipmentGrade.Rare] = rareData;
                            break;
                        case EquipmentGrade.Epic:
                        case EquipmentGrade.Epic1:
                        case EquipmentGrade.Epic2:
                            EquipmentSkillData epicData =
                                dataManager.EquipmentSkillDataDict[_equipmentSkillGroupData.EpicGradeSkill];
                            _equipmentSkillDataDict[EquipmentGrade.Epic] = epicData;
                            break;
                        case EquipmentGrade.Legendary:
                        case EquipmentGrade.Legendary1:
                        case EquipmentGrade.Legendary2:
                        case EquipmentGrade.Legendary3:
                            EquipmentSkillData legendaryData =
                                dataManager.EquipmentSkillDataDict[_equipmentSkillGroupData.LegendaryGradeSkill];
                            _equipmentSkillDataDict[EquipmentGrade.Legendary] = legendaryData;
                            break;
                    }
                }
            }
        }

        public bool IsEquipped()
        {
            return _userEquipmentData.isEquipped;
        }
        
        public int GetEquipmentStat()
        {
            int value = 0;
            
            switch(_equipmentData.EquipmentType)
            {
                case EquipmentType.Weapon:
                case EquipmentType.Gloves:
                case EquipmentType.Ring:
                    value += _equipmentData.Grade_Atk + Level * _equipmentData.LevelUp_Atk;
                    break;
                case EquipmentType.Belt:
                case EquipmentType.Armor:
                case EquipmentType.Boots:
                    value += _equipmentData.Grade_Hp + Level * _equipmentData.LevelUp_HP;
                    break;
            }

            return value;
        }

        public int GetGradeCount()
        {
            return Utils.GetGradeCount(_equipmentData.Grade);
        }

        public bool IsPossibleLevelUp()
        {
            if (IsMaxLevel)
            {
                return false;
            }
            
            //골드 + 스크롤
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            EquipmentLevelData data = Manager.I.Data.EquipmentLevelDataDict[Level];
            if (userModel.Gold.Value >= data.UpgradeCost &&
                userModel.GetScrollCount(EquipmentType) >= data.UpgradeRequiredItems)
            {
                return true;
            }

            return false;
        }
        
    }
}