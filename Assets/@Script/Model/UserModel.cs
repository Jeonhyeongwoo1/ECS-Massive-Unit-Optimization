using System;
using System.Collections.Generic;
using System.Linq;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Interface;
using UniRx;
using UnityEngine;

namespace MewVivor.Model
{
    public class UserModel : IModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public CreatureData CreatureData { get; private set; }
        public int CreatureHp => (int)CreatureData.MaxHp;
        public int CreatureAtk => (int)CreatureData.Atk;
        public Inventory Inventory => _inventory;
        public UserData UserData => _userData;
        public List<StageHistoryData> StageHistory => _stageHistory;
        public InfinityHistoryData InfinityHistory => _infinityHistory;
        public Dictionary<string, ShopSubscriptionInfoData> SubscriptionInfo => _subscriptionInfo;
        public List<MailData> Mails => _mails;
        
        public ReactiveProperty<long> AccumulatedExp;
        public ReactiveProperty<int> Stamina;
        public ReactiveProperty<int> Jewel;
        public ReactiveProperty<int> Gold;
        public ReactiveProperty<int> InfinityTicket;

        public ReactiveProperty<bool> IsPossibleGetMail = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsPossibleMerge = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsPossibleMergeAll = new ReactiveProperty<bool>();
        
        public List<Equipment> AllEquipmentList => _unEquipmentList.Concat(_equippedList).ToList();

        private ReactiveCollection<Equipment> _unEquipmentList = new ReactiveCollection<Equipment>();
        private List<Equipment> _equippedList = new();
        private List<StageHistoryData> _stageHistoryDataList = new();
        private CompositeDisposable _subscriptionDisposables;
        private CompositeDisposable _timeDisposables;
        private UserData _userData;
        private Inventory _inventory;
        private List<StageHistoryData> _stageHistory;
        private InfinityHistoryData _infinityHistory;
        private Dictionary<string, ShopSubscriptionInfoData> _subscriptionInfo;
        private List<MailData> _mails;
        
        public void Initialize(GetUserData getUserData, CreatureData creatureData)
        {
            CreatureData = creatureData;
            _userData = getUserData.userData;
            _inventory = getUserData.inventory;
            _stageHistory = getUserData.stageHistories;
            _infinityHistory = getUserData.infinityHistory;
            _subscriptionInfo = getUserData.subscriptionInfo;
            _stageHistoryDataList = getUserData.stageHistories;
            AccumulatedExp = new ReactiveProperty<long>(_userData.exp);
            Stamina = new ReactiveProperty<int>(_userData.stamina);
            Jewel = new ReactiveProperty<int>(getUserData.inventory.jewel);
            Gold = new ReactiveProperty<int>(getUserData.inventory.gold);
            InfinityTicket = new ReactiveProperty<int>(getUserData.inventory.infiniteTicket);

            if (_timeDisposables != null)
            {
                _timeDisposables.Dispose();
            }
            else
            {
                _timeDisposables = new CompositeDisposable();
            }
            
            Stamina.Subscribe(x =>
            {
                float maxChargedStamina = Manager.I.Data.GlobalConfigDataDict[GlobalConfigName.MaxChargedStamina].Value;
                if (x < maxChargedStamina)
                {
                    Debug.Log("Start stamina Timer");
                    Manager.I.Time.StaminaTimer();
                }
            }).AddTo(_timeDisposables);

            InfinityTicket.Subscribe(x =>
            {
                float maxChargedStamina = Manager.I.Data.GlobalConfigDataDict[GlobalConfigName.MaxChargedStamina].Value;
                if (x < maxChargedStamina)
                {
                    Debug.Log("Start infinity ticket Timer");
                    Manager.I.Time.InfiniteTicketTimer();
                }
            }).AddTo(_timeDisposables);
            
            if (_subscriptionDisposables == null)
            {
                _subscriptionDisposables = new CompositeDisposable();
                _unEquipmentList.ObserveAdd().Subscribe(x =>
                {
                    IsPossibleMerge.Value = CanMergeAnyUnEquipment();
                    IsPossibleMergeAll.Value = CanAllMergeUnEquipment();
                });
                _unEquipmentList.ObserveRemove().Subscribe(x =>
                {
                    IsPossibleMerge.Value = CanMergeAnyUnEquipment();
                    IsPossibleMergeAll.Value = CanAllMergeUnEquipment();
                });
            }

            MakeEquipment(getUserData.userEquipments);
            if (getUserData.mails != null && getUserData.mails.Count > 0)
            {
                SetMailData(getUserData.mails);
            }
        }

        public bool IsPossibleMergeUnEquipment(Equipment equipment, List<Equipment> list)
        {
            EquipmentData equipmentData = equipment.EquipmentData;
            if (equipmentData.MergeType == MergeType.SameItem)
            {
                var sameEquipmentList =
                    list.FindAll(v => v != equipment && v.EquipmentBaseItemId == equipmentData.ItemCode);
                if (sameEquipmentList.Count >= equipmentData.MergeNeedCount)
                {
                    return true;
                }
            }
            else if (equipmentData.MergeType == MergeType.SameGrade)
            {
                var sameEquipmentList = list.FindAll(v =>
                    v != equipment && v.Grade == equipmentData.Grade &&
                    v.EquipmentType == equipmentData.EquipmentType);
                if (sameEquipmentList.Count >= equipmentData.MergeNeedCount)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanMergeAnyUnEquipment()
        {
            List<Equipment> list = _unEquipmentList.ToList();
            foreach (Equipment equipment in list)
            {
                if (IsPossibleMergeUnEquipment(equipment, list))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanAllMergeUnEquipment()
        {
            var list = _unEquipmentList.ToList();
            foreach (Equipment equipment in list)
            {
                EquipmentData equipmentData = equipment.EquipmentData;
                if (equipmentData.Grade >= EquipmentGrade.Epic)
                {
                    continue;
                }
                
                if (IsPossibleMergeUnEquipment(equipment, list))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetUserData(GetUserData getUserData)
        {
            _stageHistory = getUserData.stageHistories;
            _infinityHistory = getUserData.infinityHistory;
            _subscriptionInfo = getUserData.subscriptionInfo;
            _stageHistoryDataList = getUserData.stageHistories;

            SetInventory(getUserData.inventory);
            MakeEquipment(getUserData.userEquipments);
            SetUserData(getUserData.userData);
            if (getUserData.mails != null && getUserData.mails.Count > 0)
            {
                SetMailData(getUserData.mails);
            }
        }

        public void SetUserInfiniteTicketAndStamina(int infiniteTicket, int stamina)
        {
            _inventory.infiniteTicket = infiniteTicket;
            _userData.stamina = stamina;
            Stamina.Value = stamina;
            InfinityTicket.Value = infiniteTicket;
        }
        
        public void SetUserData(UserData userData)
        {
            _userData = userData;
            AccumulatedExp.Value = userData.exp;
            Stamina.Value = _userData.stamina;
        }

        public void SetInventory(Inventory inventory)
        {
            Jewel.Value = inventory.jewel;
            Gold.Value = inventory.gold;
            InfinityTicket.Value = inventory.infiniteTicket;
            _inventory = inventory;
        }

        public void SetMailData(List<MailData> mailDataList)
        {
            _mails ??= new List<MailData>();
            _mails = mailDataList;
            if (_mails != null)
            {
                IsPossibleGetMail ??= new ReactiveProperty<bool>();
                IsPossibleGetMail.Value = _mails.Count > 0;
            }
        }

        public void MakeEquipment(List<UserEquipmentData> userEquipmentList)
        {
            if (userEquipmentList == null)
            {
                return;
            }
            
            _equippedList.Clear();
            _unEquipmentList.Clear();
            foreach (UserEquipmentData userEquipmentData in userEquipmentList)
            {
                EquipmentData equipmentData = Manager.I.Data.EquipmentDataDict[userEquipmentData.baseItemId];
                Equipment equipment = new Equipment(equipmentData, userEquipmentData);
                if (userEquipmentData.isEquipped)
                {
                    _equippedList.Add(equipment);
                }
                else
                {
                    _unEquipmentList.Add(equipment);
                }
            }
        }

        public Equipment AddEquipment(UserEquipmentData userEquipmentData)
        {
            EquipmentData equipmentData = Manager.I.Data.EquipmentDataDict[userEquipmentData.baseItemId];
            Equipment equipment = new Equipment(equipmentData, userEquipmentData);
            if (userEquipmentData.isEquipped)
            {
                _equippedList.Add(equipment);
            }
            else
            {
                _unEquipmentList.Add(equipment);
            }

            return equipment;
        }

        public Equipment FindEquippedItemOrUnEquippedItem(string uid)
        {
            if (_equippedList != null)
            {
                foreach (var equipment in _equippedList)
                {
                    if (equipment.UID == uid)
                    {
                        return equipment;
                    }
                }
            }

            if (_unEquipmentList != null)
            {
                foreach (var equipment in _unEquipmentList)
                {
                    if (equipment.UID == uid)
                    {
                        return equipment;
                    }
                }
            }

            Debug.LogError("Failed find equipped : " + uid);
            return null;
        }

        public bool IsOpenedStage(int stage)
        {
            var stageData = _stageHistoryDataList.Find(v => v.stage == stage);
            if (stageData != null)
            {
                return true;
            }
            
            //스테이지가 없다면 이전 스테이지가 존재하는지 확인 후 클리어 여부를 확인한다.
            var prevStage = _stageHistoryDataList.Find(v => v.stage == stage - 1);
            if (prevStage != null && prevStage.isCleared)
            {
                return true;
            }
            
            return false;
        }

        public (int hp, int atk) GetEquipmentStat()
        {
            if (_equippedList == null)
            {
                Debug.LogError("Failed find equipped");
                return (0, 0);
            }

            int hp = 0;
            int atk = 0;
            foreach (var equipment in _equippedList)
            {
                if (equipment.IsEquipped())
                {
                    hp += equipment.EquipmentData.Grade_Hp + equipment.Level * equipment.EquipmentData.LevelUp_HP;
                    atk += equipment.EquipmentData.Grade_Atk + equipment.Level * equipment.EquipmentData.LevelUp_Atk;
                }
            }

            return (hp, atk);
        }

        public (int hp, int atk) GetTotalEquipmentStat()
        {
            int totalHP = CreatureHp + GetEquipmentStat().hp;
            int totalAttackDamage = CreatureAtk + GetEquipmentStat().atk;
            return (totalHP, totalAttackDamage);
        }

        public List<Equipment> GetUnequipItemList()
        {
            return _unEquipmentList.ToList();
        }

        public List<Equipment> GetEquippedItemList()
        {
            return _equippedList;
        }

        public int GetScrollCount(EquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    return Inventory.weaponScroll;
                case EquipmentType.Gloves:
                    return Inventory.glovesScroll;
                case EquipmentType.Ring:
                    return Inventory.ringScroll;
                case EquipmentType.Belt:
                    return Inventory.beltScroll;
                case EquipmentType.Armor:
                    return Inventory.armorScroll;
                case EquipmentType.Boots:
                    return Inventory.bootsScroll;
            }

            Debug.LogError("Failed get scroll count " + equipmentType);
            return 0;
        }

        public int FindLastStageHistoryIndex()
        {
            var stage = _stageHistoryDataList[^1];
            if (stage.isCleared)
            {
                var list = Manager.I.Data.StageDict.Values.ToList();
                int stageIndex = list[^1].StageIndex;
                if (stage.stage == stageIndex)
                {
                    return stage.stage;
                }
                
                return stage.stage + 1;
            }

            return stage.stage;
        }
        
        public StageHistoryData GetStageHistory(int stageIndex)
        {
            var stage = _stageHistoryDataList.Find(v => v.stage == stageIndex);
            return stage;
        }
        
        public Equipment FindEquippedEquipment(EquipmentType equipmentType)
        {
            return _equippedList.Find(v => v.EquipmentType == equipmentType);
        }

        public void EquipOrUnEquip(UserEquipmentData currentEquippedItem, UserEquipmentData previousEquippedItem)
        {
            if (currentEquippedItem != null)
            {
                Equipment currentEquipment =
                    FindEquippedItemOrUnEquippedItem(currentEquippedItem.userEquipmentId);

                currentEquipment.SetUserEquipmentData(currentEquippedItem);
                _equippedList.Add(currentEquipment);
                _unEquipmentList.Remove(currentEquipment);
            }

            if (previousEquippedItem != null)
            {
                Equipment previousEquipment =
                    FindEquippedItemOrUnEquippedItem(previousEquippedItem.userEquipmentId);
                previousEquipment.SetUserEquipmentData(previousEquippedItem);
                _equippedList.Remove(previousEquipment);
                _unEquipmentList.Add(previousEquipment);
            }
        }

        public int GetEvolutionLevel(EvolutionType evolutionType)
        {
            int level = 0;
            switch (evolutionType)
            {
                case EvolutionType.Atk:
                    level = _userData.evolutionAtkCount;
                    break;
                case EvolutionType.MaxHp:
                    level = _userData.evolutionHpCount;
                    break;
                case EvolutionType.MoveSpeed:
                    level= _userData.evolutionMoveCount;
                    break;
                case EvolutionType.CriticalPercent:
                    level = _userData.evolutionCriticalCount;
                    break;
                case EvolutionType.CriticalDamage:
                    level = _userData.evolutionCriticalDamageCount;
                    break;
                case EvolutionType.Boom:
                    level= _userData.evolutionBoomCount;
                    break;
                case EvolutionType.SkillCoolDown:
                    level = _userData.evolutionSkillCoolCount;
                    break;
                case EvolutionType.Berserker:
                    level = _userData.evolutionBerserkerCount;
                    break;
                case EvolutionType.invincibility:
                    level = _userData.evolutionInvincibilityCount;
                    break;
            }

            return level;
        }

        public bool HasEvolution(EvolutionType evolutionType)
        {
            return GetEvolutionLevel(evolutionType) >= 1;
        }

        public (float evolutionValue1, float evolutionValue2) CalculateEvolutionStat(EvolutionType evolutionType)
        {
            int level = GetEvolutionLevel(evolutionType);
            foreach (var (key, evolutionData) in Manager.I.Data.EvolutionDataDict)
            {
                if (evolutionData.EvolutionType == evolutionType)
                {
                    switch (evolutionData.EvolutionType)
                    {
                        case EvolutionType.Atk:
                        case EvolutionType.MaxHp:
                        case EvolutionType.Boom:
                        case EvolutionType.invincibility:
                        case EvolutionType.Berserker:
                        case EvolutionType.MoveSpeed:
                        case EvolutionType.CriticalPercent:
                        case EvolutionType.CriticalDamage:
                        case EvolutionType.SkillCoolDown:
                            break;
                    }
                    
                    float evolutionValue1 = (evolutionData.DefaultEvolutionValue1 +
                                                    (level - 1) * evolutionData.LevelUpEvolutionValue1);
                    float evolutionValue2 = (evolutionData.DefaultEvolutionValue2 +
                                                    (level - 1) * evolutionData.LevelUpEvolutionValue2);
                    return (evolutionValue1, evolutionValue2);
                }
            }

            Debug.LogError("Failed evolution" + evolutionType);
            return (0, 0);
        }

        public (float defaultEvolutionValue1, float defaultEvolutionValue2) GetDefaultEvolutionValue(
            EvolutionOrderType evolutionOrderType)
        {
            float defaultEvolutionValue1 = 0;
            float defaultEvolutionValue2 = 0;
            EvolutionData evolutionData = Manager.I.Data.EvolutionDataDict[evolutionOrderType.ToString()];
            defaultEvolutionValue1 = evolutionData.DefaultEvolutionValue1;
            defaultEvolutionValue2 = evolutionData.DefaultEvolutionValue2;
            return (defaultEvolutionValue1, defaultEvolutionValue2);
        }

        public int GetStageBestAliveTime(int stageIndex)
        {
            var stageHistory = _stageHistory.Find(v => v.stage == stageIndex);
            if (stageHistory != null)
            {
                return stageHistory.survivalTime;
            }
            else
            {
                stageHistory = _stageHistory.Find(v => v.stage == stageIndex - 1);
                return stageHistory?.survivalTime ?? 0;
            }
        }

        public int GetInfiniteBestAliveTime()
        {
            return _infinityHistory.survivalTime;
        }

        public void Reset()
        {
            _unEquipmentList = new ReactiveCollection<Equipment>();
            _equippedList = new();
            _stageHistoryDataList = new();
            _subscriptionDisposables?.Dispose();
            _timeDisposables?.Dispose();
            _userData = null;
            _inventory = null;
            _stageHistory = null;
            _infinityHistory = null;
            _subscriptionInfo = null;
            _mails = null;
        }
    }
}