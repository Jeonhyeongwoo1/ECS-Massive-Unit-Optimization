using System;
using System.Collections.Generic;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Controller;
using MewVivor.Model;
using UnityEngine;

namespace MewVivor.InGame.Stat
{
    public enum ModifyType
    {
        Flat, //+10
        PercentAdd, //10%
        PercentMul, // *1.1f
    }

    public class StatModifer
    {
        public float Value;
        public int Order;
        public ModifyType ModifyType;
        public object Source; //이 버프의 출처
        
        public StatModifer(float value, ModifyType modifyType, object source, int order = 1)
        {
            Value = value;
            ModifyType = modifyType;
            Order = order;
            Source = source;
        }

        public void UpdateValue(float value)
        {
            Value = value;
        }
    }
    
    [Serializable]
    public class CreatureStat
    {
        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    CalculateStat();
                    _isDirty = false;
                }

                return _value;
            }
        }

        protected bool _isDirty;
        protected float _originValue;
        protected float _value;
        protected List<StatModifer> _statModiferList = new();
        protected CreatureStatType _creatureStatType;
        protected PlayerModel _playerModel;
        protected CreatureController _owner;

        public CreatureStat(float originValue, CreatureStatType creatureStatType, CreatureController owner)
        {
            _originValue = originValue;
            _value = _originValue;
            _owner = owner;
            _creatureStatType = creatureStatType;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            UpdatePlayerStatData();
        }

        protected void UpdatePlayerStatData()
        {
            if (_owner is PlayerController)
            {
                _playerModel.UpdatePlayerStateData(_creatureStatType, _value);
            }
        }

        public void AddModifier(StatModifer statModifer)
        {
            if (_statModiferList.Contains(statModifer))
            {
                _statModiferList.Remove(statModifer);
            }
            
            _statModiferList.Add(statModifer);
            _isDirty = true;
            
            //Debug..
            CalculateStat();
        }

        public void RemoveModifer(StatModifer statModifer)
        {
            _statModiferList.Remove(statModifer);
            _isDirty = true;
            
            //Debug..
            CalculateStat();
        }

        public void ForceCalculateStat()
        {
            CalculateStat();
            _isDirty = false;
        }

        public void RemoveAll()
        {
            _statModiferList.Clear();
        }
        
        public void RemoveAllModifiersFromSource(object source)
        {
            _statModiferList.RemoveAll(m => m.Source == source);
            _isDirty = true;
        }

        protected virtual void CalculateStat()
        {
            float finalValue = _originValue;
            float sumPercent = 0;
            
            foreach (StatModifer statModifer in _statModiferList)
            {
                switch (statModifer.ModifyType)
                {
                    case ModifyType.Flat:
                        finalValue += statModifer.Value;
                        break;
                    case ModifyType.PercentAdd:
                        sumPercent += statModifer.Value;
                        break;
                    case ModifyType.PercentMul:
                        finalValue *= (1 + statModifer.Value);
                        break;
                }
            }

            finalValue *= (1 + sumPercent);
            _value = finalValue;
            UpdatePlayerStatData();
        }

    }

    [Serializable]
    public class ChanceStat : CreatureStat
    {
        public ChanceStat(float originValue, CreatureStatType creatureStatType, CreatureController owner) : base(originValue, creatureStatType, owner)
        {
            _originValue = originValue;
            _value = _originValue;
            _owner = owner;
            _creatureStatType = creatureStatType;
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            UpdatePlayerStatData();
        }

        protected override void CalculateStat()
        {
            float finalValue = _originValue;
            float sumPercent = 0;
            
            foreach (StatModifer statModifer in _statModiferList)
            {
                switch (statModifer.ModifyType)
                {
                    case ModifyType.Flat:
                        finalValue += statModifer.Value;
                        break;
                    case ModifyType.PercentAdd:
                        sumPercent += statModifer.Value;
                        break;
                    case ModifyType.PercentMul:
                        finalValue *= (1 + statModifer.Value);
                        break;
                }
            }

            _value = Mathf.Clamp01(sumPercent);
            UpdatePlayerStatData();
        }
    }
}