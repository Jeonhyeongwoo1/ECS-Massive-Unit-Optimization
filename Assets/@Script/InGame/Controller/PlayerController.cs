using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lofelt.NiceVibrations;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Equipmenets;
using MewVivor.Factory;
using MewVivor.InGame.Stat;
using MewVivor.InGame.Entity;
using MewVivor.InGame.Input;
using MewVivor.InGame.Skill;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Presenter;
using UnityEngine;
using UniRx;
using Random = UnityEngine.Random;

namespace MewVivor.InGame.Controller
{
    public partial class PlayerController : CreatureController
    {
        public Transform AttackPoint => _attackPoint;
        public int Layer => _layer;

        public CreatureStat ExpStat { get; private set; }
        public CreatureStat Defense { get; private set; }
        public CreatureStat GoldAmountPercent { get; private set; }
        public CreatureStat MagneticRangePercent { get; private set; }
        public CreatureStat SkillCoolTime { get; private set; }
        public CreatureStat AuraDamageUpPercent { get; private set; }
        public CreatureStat ExplosionSkillSize { get; private set; }
        public CreatureStat BulletSkillSize { get; private set; }
        public CreatureStat CircleSkillSize { get; private set; }
        public CreatureStat ItemBoxSpawnCoolTime { get; private set; }
        public CreatureStat BasicSkillCoolTime { get; private set; }
        public CreatureStat HpRecoveryEfficiency { get; private set; }
        public CreatureStat InvincibleChance { get; private set; }
        public CreatureStat BombStat { get; private set; }
        
        public float CurrentExp
        {
            get => _playerModel.CurrentExp.Value;
            set
            {
                float exp = ExpStat.Value * Mathf.RoundToInt(value);
                _playerModel.CurrentExp.Value += (int) exp;
                var levelData = Manager.I.Data.LevelDataDict[_playerModel.CurrentLevel.Value];
                _playerModel.CurrentExpRatio.Value = (float)_playerModel.CurrentExp.Value / levelData.AccumulatedEXP;
                if (_playerModel.CurrentExp.Value >= levelData.AccumulatedEXP)
                {
                    OnLevelUp();
                    _playerModel.CurrentExp.Value -= levelData.AccumulatedEXP;
                }
            }
        }

        public PlayerStateType PlayerStateType => _playerStateType;
        public StatModifer BerserkerStatModifer { get; private set; }
        
        [SerializeField] private Transform _indicatorTransform;
        [SerializeField] private MoveComponent _moveComponent;
        [SerializeField] private Transform _attackPoint;

        private readonly float MagneticRange = 1f;
        private int _layer;
        private Vector2 _inputVector;
        private PlayerModel _playerModel;
        private UserModel _userModel;
        private PlayerStateType _playerStateType;
        private CancellationTokenSource _recoveryHPCts;
        private CancellationTokenSource _resurrectionCts;


        public override void Initialize(CreatureData creatureData,
            List<AttackSkillData> skillDataList)
        {
            _playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            _userModel = ModelFactory.CreateOrGetModel<UserModel>();
            _creatureType = CreatureType.Player;

            base.Initialize(creatureData, skillDataList);

            transform.localScale = Vector3.one * creatureData.Scale;
            InitializeEquipmentSkill();
            InitializeEvolution();
            _moveComponent.Initialize(_rigidbody, _indicatorTransform, _pivotTransform, MoveSpeed.Value);
            HP.Subscribe(CalculateAtkPercentByBerserker).AddTo(this);
            gameObject.SetActive(true);
        }

        private void CalculateAtkPercentByBerserker(float currentHP)
        {
            if (BerserkerStatModifer == null)
            {
                return;
            }
            
            (float evolutionValue1, float evolutionValue2) = _userModel.CalculateEvolutionStat(EvolutionType.Berserker);
            float maxHP = MaxHP.Value;
            float targetHP = evolutionValue1;
            float delta = maxHP - currentHP;
            float mod = delta / targetHP;
            if (mod < 1)
            {
                mod = 0;
            }

            float finalValue = mod * evolutionValue2;
            BerserkerStatModifer.UpdateValue(finalValue);
            Atk.ForceCalculateStat();
            // Debug.Log($"final value mod {mod} / {evolutionValue2} / final berserker {finalValue} / final ATk {Atk.Value}");
        }

        protected override void InitCreatureStat(bool isFullHP = true)
        {
            (int totalHP, int totalAttackDamage) = _userModel.GetTotalEquipmentStat();
            MaxHP ??= new CreatureStat(totalHP, CreatureStatType.MaxHP, this);
            Atk ??= new CreatureStat(totalAttackDamage, CreatureStatType.Atk, this);

            MoveSpeed ??= new CreatureStat(CreatureData.MoveSpeed, CreatureStatType.MoveSpeed, this);
            CriticalPercent ??= new CreatureStat(CreatureData.CriticalPercent, CreatureStatType.CriticalPercent, this);
            CriticalDamagePercent ??= new CreatureStat(CreatureData.CriticalDamagePercent,
                CreatureStatType.CriticalDamagePercent, this);
            HpRecovery ??= new CreatureStat(CreatureData.HpRecovery, CreatureStatType.HpRecovery, this);

            ExpStat ??= new CreatureStat(1, CreatureStatType.Exp, this);
            Defense ??= new ChanceStat(0, CreatureStatType.Defense, this);
            GoldAmountPercent ??= new CreatureStat(1, CreatureStatType.AddGoldAmount, this);
            MagneticRangePercent ??= new CreatureStat(1, CreatureStatType.MagneticRangePercent, this);
            
            //Equipment stat
            SkillCoolTime ??= new CreatureStat(0, CreatureStatType.SkillCoolTime, this);
            HpRecoveryEfficiency ??= new CreatureStat(1, CreatureStatType.HpRecoveryEfficiency, this);
            AuraDamageUpPercent ??= new CreatureStat(0, CreatureStatType.AuraDamageUpPercent, this);
            ExplosionSkillSize ??= new CreatureStat(1, CreatureStatType.ExplosionSkillSize, this);
            BulletSkillSize ??= new CreatureStat(1, CreatureStatType.BulletSkillSize, this);
            CircleSkillSize ??= new CreatureStat(1, CreatureStatType.CircleSkillSize, this);
            BasicSkillCoolTime ??= new CreatureStat(0, CreatureStatType.BasicSkillCoolTime, this);
            ItemBoxSpawnCoolTime ??= new CreatureStat(1, CreatureStatType.ItemBoxSpawnCoolTime, this);
            
            //Evolution stat
            InvincibleChance ??= new ChanceStat(0,  CreatureStatType.InvincibleChance, this);
            
            if (isFullHP)
            {
                HP.Value = MaxHP.Value;
                _playerModel.CurrentHP.Value = HP.Value;
            }
        }

        private void InitializeEvolution()
        {
            int count = System.Enum.GetValues(typeof(EvolutionType)).Length;
            for (int i = 0; i < count; i++)
            {
                EvolutionType evolutionType = (EvolutionType)i;
                (float evolutionValue1, float evolutionValue2) =
                    _userModel.CalculateEvolutionStat(evolutionType);

                switch (evolutionType)
                {
                    case EvolutionType.Atk:
                        if (_userModel.HasEvolution(EvolutionType.Atk))
                        {
                            Atk.AddModifier(new StatModifer(evolutionValue1, ModifyType.Flat, this));
                        }
                        break;
                    case EvolutionType.MaxHp:
                        if (_userModel.HasEvolution(EvolutionType.MaxHp))
                        {
                            MaxHP.AddModifier(new StatModifer(evolutionValue1, ModifyType.Flat, this));
                            HP.Value = MaxHP.Value;
                        }
                        break;
                    case EvolutionType.MoveSpeed:
                        if (_userModel.HasEvolution(EvolutionType.MoveSpeed))
                        {
                            MoveSpeed.AddModifier(new StatModifer(evolutionValue1, ModifyType.PercentAdd, this));
                        }
                        break;
                    case EvolutionType.CriticalPercent:
                        if (_userModel.HasEvolution(EvolutionType.CriticalPercent))
                        {
                            CriticalPercent.AddModifier(new StatModifer(evolutionValue1, ModifyType.PercentAdd,
                                this));
                        }
                        break;
                    case EvolutionType.CriticalDamage:
                        if (_userModel.HasEvolution(EvolutionType.CriticalDamage))
                        {
                            CriticalDamagePercent.AddModifier(new StatModifer(evolutionValue1,
                                ModifyType.PercentAdd, this));
                        }
                        break;
                    case EvolutionType.Boom:
                        if (_userModel.HasEvolution(EvolutionType.Boom))
                        {
                            BombStat = new CreatureStat(evolutionValue2, CreatureStatType.ExplosionBomb, this);
                        }
                        break;
                    case EvolutionType.SkillCoolDown:
                        if (_userModel.HasEvolution(EvolutionType.SkillCoolDown))
                        {
                            SkillCoolTime.AddModifier(new StatModifer(evolutionValue1, ModifyType.PercentAdd,
                                this));
                        }
                        break;
                    case EvolutionType.Berserker:
                        
                        /*
                         * 잃은 체력량(정수로 계산)에 따라 공격력 비율로 증가함.
                            - 잃은 체력 n당 공격력이 N% 증가함.
                            - 체력이 회복되면, 회복된 수치만큼 공격력이 감소함.
                            - 중복 성장 시, 공격력 증가율이 커짐.
                            - 다른 공격력 증가 퍼센트와는 합연산으로 계산함.
                         */
                        //체력에 따라 변화를 준다.
                        if (_userModel.HasEvolution(EvolutionType.Berserker))
                        {
                            BerserkerStatModifer = new StatModifer(0, ModifyType.PercentAdd, this);
                            Atk.AddModifier(BerserkerStatModifer);
                        }
                        break;
                    case EvolutionType.invincibility:
                        if (_userModel.HasEvolution(EvolutionType.invincibility))
                        {
                            InvincibleChance.AddModifier(new StatModifer(evolutionValue2, ModifyType.PercentAdd,
                                this));
                        }
                        break;
                }
            }
        }
        private void InitializeEquipmentSkill()
        {
            List<Equipment> list = _userModel.GetEquippedItemList();
            foreach (Equipment equipment in list)
            {
                foreach (var (grade, skillData) in equipment.EquipmentSkillDataDict)
                {
                    switch (skillData.EquipmentSkillType)
                    {
                        case EquipmentSkillType.Power:
                            Atk.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.SkillCoolTime:
                            SkillCoolTime.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.PercentAdd,
                                this));
                            break;
                        case EquipmentSkillType.MaxHP:
                            MaxHP.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.PercentAdd,
                                this));
                            HP.Value = MaxHP.Value;
                            break;
                        case EquipmentSkillType.ResistDamage:
                            Defense.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.PercentAdd,
                                this));
                            break;
                        case EquipmentSkillType.Critical:
                            CriticalPercent.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.PercentAdd,
                                this));
                            break;
                        case EquipmentSkillType.CriticalDamage:
                            CriticalDamagePercent.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.HpRecoveryEfficiency:
                            HpRecoveryEfficiency.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.MoveSpeed:
                            MoveSpeed.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.AuraDamageUp:
                            AuraDamageUpPercent.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.ExplosionSkillSize:
                            ExplosionSkillSize.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.ItemBoxCool:
                            ItemBoxSpawnCoolTime.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.BasicAttackCool:
                            BasicSkillCoolTime.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.BulletSkillSize:
                            BulletSkillSize.AddModifier(new StatModifer(skillData.Type_Value1,
                                ModifyType.PercentAdd, this));
                            break;
                        case EquipmentSkillType.CircleSkillSize:
                            CircleSkillSize.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.PercentAdd, this));
                            break;
                        //오라 생성
                        case EquipmentSkillType.AuraDamage:
                            _skillBook.AddActivateEquipmentSkillList(skillData);
                            break;
                        //체력 100% 부활 1회
                        case EquipmentSkillType.Revive:
                            _playerModel.ReviveCountBySkill.Value++;
                            break;
                        //부활 1회 
                        case EquipmentSkillType.AddRevive:
                            _playerModel.ReviveCountBySkill.Value += 2;
                            break;
                        //회복속도
                        case EquipmentSkillType.HpRecovery1s:
                            HpRecovery.AddModifier(new StatModifer(skillData.Type_Value1, ModifyType.Flat,
                                this));
                            break;
                    }
                }
            }
        }

        private void Start()
        {
            _layer = gameObject.layer;
            TryGetComponent(out _rigidbody);
            RecoveryHpAsync().Forget();
        }

        private void OnLevelUp()
        {
            var skillSelectPresenter = PresenterFactory.CreateOrGet<SkillSelectPresenter>();
            skillSelectPresenter.OpenSkillSelectPopup(true);
        }

        private async UniTaskVoid RecoveryHpAsync()
        {
            _recoveryHPCts = new CancellationTokenSource();
            while (!_recoveryHPCts.IsCancellationRequested)
            {
                Heal(HpRecovery.Value);

                try
                {
                    await UniTask.WaitForSeconds(1, cancellationToken: _recoveryHPCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogError($"error {nameof(RecoveryHpAsync)} message {e.Message}");
                    break;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            InputHandler.onInputAction += OnChangedInputVector;
            Manager.I.Event.AddEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
            // Manager.I.Event.AddEvent(GameEventType.UpgradeOrAddNewSkill, OnUpgradeOrAddNewSkill);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Utils.SafeCancelCancellationTokenSource(ref _recoveryHPCts);
            InputHandler.onInputAction -= OnChangedInputVector;
            Manager.I.Event.RemoveEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
            // Manager.I.Event.RemoveEvent(GameEventType.UpgradeOrAddNewSkill, OnUpgradeOrAddNewSkill);
        }

        protected override void Update()
        {
            if (IsDead)
            {
                return;
            }

            GetDropItem();
        }

        private void OnActivateDropItem(object value)
        {
            DropItemController item = (DropItemController)value;
            DropItemData dropItemData = item.DropItemData;

            switch (dropItemData.DropItemType)
            {
                case DropableItemType.Potion:
                    float ratio = 0;
                    if (HpRecoveryEfficiency != null)
                    {
                        ratio = dropItemData.Value * HpRecoveryEfficiency.Value;
                    }
                    else
                    {
                        ratio = dropItemData.Value;
                    }

                    ratio = Mathf.Min(1, ratio);
                    float healAmount = MaxHP.Value * ratio;
                    Heal(healAmount);
                    break;
                case DropableItemType.SkillUp:
                    var skillSelectPresenter = PresenterFactory.CreateOrGet<SkillSelectPresenter>();
                    skillSelectPresenter.OpenSkillSelectPopup(false);
                    break;
                case DropableItemType.Jewel:
                    _playerModel.Jewel.Value += (int)dropItemData.Value;
                    break;
                case DropableItemType.Gold:
                    int goldAmount = (int)(dropItemData.Value * GoldAmountPercent.Value);
                    _playerModel.Gold.Value += goldAmount;
                    break;
            }
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            if (IsDead || _playerStateType == PlayerStateType.Invincibility)
            {
                return;
            }

            if (InvincibleChance != null)
            {
                if (Random.value < InvincibleChance.Value)
                {
                    //무적
                    (float , float) tuple = _userModel.CalculateEvolutionStat(EvolutionType.invincibility);
                    float duration = tuple.Item1;
                    DOVirtual.DelayedCall(duration, () =>
                    {
                        UpdatePlayerState(PlayerStateType.Normal);
                    });
                }
            }

            float defense = Mathf.Clamp01(Defense.Value);
            damage *= (1 - defense);
            base.TakeDamage(damage, attacker);
            _playerModel.CurrentHP.Value = HP.Value;
            onHitReceived?.Invoke((int)HP.Value, (int)MaxHP.Value);
            if (Manager.I.IsOnHaptic)
            {
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            }
        }
        
        public void UpdatePlayerState(PlayerStateType playerStateType)
        {
            _playerStateType = playerStateType;
        }

        protected override void Dead()
        {
            if (IsDead)
            {
                return;
            }
         
            base.Dead();
            _skillBook?.StopAllSkillLogic();
            _moveComponent.SetStop(true);

            if (_playerModel.ReviveCountBySkill.Value > 0)
            {
                _playerModel.ReviveCountBySkill.Value--;
                DOVirtual.DelayedCall(2, () =>
                {
                    Manager.I.Event.Raise(GameEventType.UseResurrection, true);
                });
            }
            else
            {
                Manager.I.Event.Raise(GameEventType.DeadPlayer);
            }
        }

        private void OnChangedInputVector(Vector2 input)
        {
            if (IsDead)
            {
                return;
            }
            
            _inputVector = input;
            _moveComponent.SetDirection(_inputVector);
            if (_inputVector == Vector2.zero)
            {
                _indicatorTransform.gameObject.SetActive(false);
                // GetDirection/()
                UpdateCreatureState(CreatureStateType.Idle);
            }
            else
            {
                _indicatorTransform.gameObject.SetActive(true);
                UpdateCreatureState(CreatureStateType.Move);
            }
        }

        public void OnUpgradeOrAddNewSkill(int skillId, bool isLevelUp)
        {
            int id = skillId;
            DataManager dataManager = Manager.I.Data;
            if (dataManager.AttackSkillDict.TryGetValue(id, out AttackSkillData attackSkillData))
            {
                BaseSkill baseSkill = _skillBook.UpgradeOrAddAttackSkill(attackSkillData);
            }
            else
            {
                PassiveSkillData skillData = Manager.I.Data.PassiveSkillDataDict[id];
                BasePassiveSkill baseSkill = _skillBook.AddPassiveSkill(skillData);
                if (baseSkill != null)
                {
                    StatModifer statModifer = _skillBook.GetPassiveSkillStatModifer(skillData.PassiveSkillType);
                    switch (skillData.PassiveSkillType)
                    {
                        case PassiveSkillType.MoveSpeed:
                            MoveSpeed.AddModifier(statModifer);
                            _moveComponent.SetMoveSpeed(MoveSpeed.Value);
                            break;
                        case PassiveSkillType.HpRecoveryEfficiency:
                            HpRecovery.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.MaxHP:
                            MaxHP.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.CriticalPercent:
                            CriticalPercent.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.CriticalDamage:
                            CriticalDamagePercent.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.AddGoldAmount:
                            GoldAmountPercent.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.Defence:
                            Defense.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.AddItemRange:
                            MagneticRangePercent.AddModifier(statModifer);
                            break;
                        case PassiveSkillType.EXP:
                            ExpStat.AddModifier(statModifer);
                            break;
                    }

                    _skillBook.GetPassiveSkillStatModifer(baseSkill.PassiveSkillType);
                }
            }

            if (isLevelUp)
            {
                LevelUp();
            }

            //Editor
            Manager.I.Event.Raise(GameEventType.LearnSkill, _skillBook);
        }

        public void LevelUp()
        {
            _playerModel.CurrentLevel.Value++;
            CurrentExp = 0;
        }

        public bool RemoveAttackSkill(int skillId)
        {
            DataManager dataManager = Manager.I.Data;
            if (dataManager.AttackSkillDict.TryGetValue(skillId, out AttackSkillData attackSkillData))
            {
                return _skillBook.TryRemoveAttackSKill(attackSkillData);
            }

            return false;
        }

        public override void UpdateStateAndAnimation(CreatureStateType stateType, string animationName)
        {
            base.UpdateStateAndAnimation(stateType, animationName);

            switch (_creatureStateType)
            {
                case CreatureStateType.Idle:
                    _animator.SetBool(Const.AnimationName.Walk, false);
                    break;
                case CreatureStateType.Move:
                    _animator.SetBool(Const.AnimationName.Walk, true);
                    break;
                case CreatureStateType.Skill:
                    break;
                case CreatureStateType.Dead:
                    _animator.SetTrigger(Const.AnimationName.Die);
                    break;
            }
        }
        
        private void UpdatePlayerStat(bool isFullHP = true)
        {
            InitCreatureStat(isFullHP);
        }

        public override Vector3 GetDirection()
        {
            if (_inputVector == Vector2.zero)
            {
                return _indicatorTransform.right;
            }

            return _inputVector;
        }

        public List<TotalDamageInfoData> GetTotalDamageInfoData()
        {
            float totalDamage = GetTotalDamage();
            List<TotalDamageInfoData> list = new List<TotalDamageInfoData>();
            foreach (BaseAttackSkill skill in _skillBook.ActivateAttackSkillList)
            {
                if (skill.AttackSkillData.AttackSkillType == AttackSkillType.Skill_10101)
                {
                    continue;
                }

                string title = Manager.I.Data.LocalizationDataDict[skill.AttackSkillData.TitleTextKey].GetValueByLanguage();
                var infoData = new TotalDamageInfoData
                {
                    skillSprite = Manager.I.Resource.Load<Sprite>(skill.AttackSkillData.IconLabel),
                    skillName = title,
                    skillDamageRatioByTotalDamage = totalDamage > 0 && skill.AccumulatedDamage > 0
                        ? skill.AccumulatedDamage / totalDamage
                        : 0,
                    skillAccumlatedDamage = skill.AccumulatedDamage
                };
                
                list.Add(infoData);
            }

            return list.OrderBy((a) => a.skillAccumlatedDamage).ToList();
        }
        
        public void Heal(float healAmount)
        {
            HP.Value = Mathf.Min(HP.Value + healAmount, MaxHP.Value);
            onHealReceived?.Invoke((int)HP.Value, (int)MaxHP.Value);
        }

        private float GetTotalDamage()
        {
            float damage = 0;
            foreach (BaseAttackSkill skill in _skillBook.ActivateAttackSkillList)
            {
                damage += skill.AccumulatedDamage;
            }

            return damage;
        }
        
        private void GetDropItem()
        {
            GridController grid = Manager.I.Game.CurrentMapController.Grid;
            List<DropItemController> itemControllerList =
                grid.GetDropItem(transform.position, 
                            Const.DEFAULT_MagneticRange * MagneticRangePercent.Value);
            
            foreach (DropItemController item in itemControllerList)
            {
                switch (item.DropableItemType)
                {
                    case DropableItemType.Gem:
                        Action callback = () =>
                        {
                            var gem = item as GemController;
                            int exp = gem.GetExp();
                            CurrentExp = exp;
                        };     
                        item.GetItem(transform, false, callback);
                        break;
                    case DropableItemType.Potion:
                    case DropableItemType.Magnet:
                    case DropableItemType.Bomb:
                    case DropableItemType.SkillUp:
                    case DropableItemType.Gold:
                    case DropableItemType.Jewel:
                        item.GetItem(transform);
                        break;
                    case DropableItemType.ItemBox:
                        continue;
                }
                
                grid.RemoveItem(item);
            }
        }
        
        //부활
        public async UniTask DoResurrectionAsync()
        {
            UpdatePlayerStat();
            UpdatePlayerState(PlayerStateType.Invincibility);
            _resurrectionCts = new CancellationTokenSource();
            onHitReceived?.Invoke((int)HP.Value, (int)MaxHP.Value);
            GameObject particleObject = Manager.I.Resource.Instantiate(Const.Revival, false);
            particleObject.transform.SetParent(transform);
            particleObject.transform.localPosition = Vector3.zero;
            _animator.SetTrigger(Const.AnimationName.Idle);
            await UniTask.WaitForSeconds(1f, cancellationToken: _resurrectionCts.Token);
            
            particleObject.SetActive(true);
            Destroy(particleObject);
            
            _moveComponent.SetStop(false);
            _skillBook.UseAllSkillList(true, false, null);
            UpdateCreatureState(CreatureStateType.Idle);
            await UniTask.WaitForSeconds(Const.PlayerInvincibilityTime, cancellationToken: _resurrectionCts.Token);
            UpdatePlayerState(PlayerStateType.Normal);
        }
        
        private void OnActivateAI()
        {
            var ai = gameObject.GetComponent<PlayerAI>();
            ai.Activate();
        }

        #region Cheat

        public void Cheat_ResizeScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        #endregion
    }
}