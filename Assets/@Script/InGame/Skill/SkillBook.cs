using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Stat;
using MewVivor.Managers;
using MewVivor.Model;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class SkillBook
    {
        public List<BaseAttackSkill> ActivateAttackSkillList => _activateAttackSkillList;

        public List<BaseSkill> AllActivateSkillList => _activateAttackSkillList.Cast<BaseSkill>()
            .Concat(_activatePassiveSkillList).ToList();

        public List<BasePassiveSkill> ActivatePassiveSkillList => _activatePassiveSkillList;

        private List<BaseAttackSkill> _activateAttackSkillList = new();
        private List<BasePassiveSkill> _activatePassiveSkillList = new();
        private List<EquipmentSkill> _activateEquipmentSkillList = new();

        private CancellationTokenSource _useSequenceSkillCts;
        private CreatureController _owner;

        private List<AttackSkillData> AttackSkillDataList => _attackSkillDataList ??= Utils.CreateDefaultSkillData();

        private List<PassiveSkillData> PassiveSkillDataList =>
            _passiveSkillDataList ??= Utils.CreateDefaultPassiveSkillData();

        //기본 타입들에 대한 정보를 저장한다.
        private List<AttackSkillData> _attackSkillDataList;
        private List<PassiveSkillData> _passiveSkillDataList;

        public SkillBook(CreatureController owner, List<AttackSkillData> skillList)
        {
            _owner = owner;
            AddSkillList(skillList);
        }

        private void AddSkillList(List<AttackSkillData> skillList)
        {
            if (skillList == null)
            {
                return;
            }

            //미리 모든 스킬들을 초기화 시켜서 셋업
            foreach (AttackSkillData skillData in skillList)
            {
                BaseAttackSkill attackSkill = AddAttackSkill(skillData);
                UseSkill(attackSkill);
            }
        }

        public BaseSkill UpgradeOrAddAttackSkill(AttackSkillData attackSkillData)
        {
            if (attackSkillData == null)
            {
                Debug.LogError("SkillData is null");
                return null;
            }

            BaseAttackSkill skill = _activateAttackSkillList
                .Find(v => v.AttackSkillData.AttackSkillType == attackSkillData.AttackSkillType);
            if (skill == null)
            {
                skill = AddAttackSkill(attackSkillData);
                if (skill == null)
                {
                    Debug.LogError($"Failed get skill {attackSkillData.DataId}");
                    return null;
                }

                UseSkill(skill);
                return skill;
            }

            if (skill.IsMaxLevel)
            {
                Debug.LogWarning("Max level :" + skill.AttackSkillData.DataId);
                return null;
            }

            AttackSkillData upgradeAttackSkillData = Manager.I.Data.AttackSkillDict[attackSkillData.DataId + 1];
            skill.LevelUp(upgradeAttackSkillData);
            UseSkill(skill);
            return skill;
        }

        public bool TryRemoveAttackSKill(AttackSkillData attackSkillData)
        {
            var skill = _activateAttackSkillList.Find(v => v.AttackSkillData.DataId == attackSkillData.DataId);
            if (skill == null)
            {
                Debug.LogError($"Failed remove skill {attackSkillData.DataId}");
                return false;
            }

            skill.StopSkillLogic();
            _activateAttackSkillList.Remove(skill);
            return true;
        }

        private AttackSkillType GetAttackSkillType(int id)
        {
            return (AttackSkillType)id;

            // SkillType skillType = SkillType.None;
            // if (id > (int)SkillType.BossSkill)
            // {
            //     Array array = System.Enum.GetValues(typeof(SkillType));
            //     foreach (var obj in array)
            //     {
            //         int value = (int)obj;
            //         int range = 4 + value;
            //         if (value <= id && id <= range)
            //         {
            //             return (SkillType)value;
            //         }
            //     }
            // }
            // else
            // {
            //     return (SkillType)id;
            // }
            //
            // Debug.LogError("Failed get skill type : " + id);
            // return SkillType.None;
        }

        private BaseAttackSkill AddAttackSkill(AttackSkillData attackSkillData)
        {
            AttackSkillType attackSkillType = GetAttackSkillType(attackSkillData.DataId);
            if (attackSkillType == AttackSkillType.None)
            {
                Debug.LogWarning("SkillType is None / skill id : " + attackSkillData.DataId);
                return null;
            }

            string skillName = $"{typeof(BaseAttackSkill).Namespace}.{attackSkillType}";
            var skill = Activator.CreateInstance(Type.GetType(skillName)) as BaseAttackSkill;
            if (skill == null)
            {
                Debug.LogWarning("skill is null : " + attackSkillData.DataId);
                return null;
            }

            skill.Initialize(_owner, attackSkillData);
            _activateAttackSkillList.Add(skill);
            return skill;
        }

        public BasePassiveSkill AddPassiveSkill(PassiveSkillData skillData)
        {
            BasePassiveSkill passiveSkill = _activatePassiveSkillList.Find(v => v.SkillData.DataId == skillData.DataId);
            if (passiveSkill == null)
            {
                var skill = new BasePassiveSkill();
                skill.Initialize(skillData);
                _activatePassiveSkillList.Add(skill);
                return skill;
            }

            if (passiveSkill.IsMaxLevel)
            {
                Debug.LogWarning($"skill is maxLevel {skillData.DataId}");
                return null;
            }

            passiveSkill.LevelUp(skillData);
            return passiveSkill;
        }

        public bool IsContainLinkedSkillId(PassiveSkillType passiveSkillType, int skillId)
        {
            BasePassiveSkill passiveSkill = _activatePassiveSkillList.Find(v => v.PassiveSkillType == passiveSkillType);
            if (passiveSkill == null)
            {
                return false;
            }

            return passiveSkill.IsLinkedSkill(skillId);
        }

        public StatModifer GetPassiveSkillStatModifer(PassiveSkillType passiveSkillType)
        {
            BasePassiveSkill skill = _activatePassiveSkillList.Find(v => v.PassiveSkillType == passiveSkillType);
            return skill?.PassiveStatModifer;
        }

        #region UseSkill

        private void UseSkill(BaseAttackSkill baseAttackSkill)
        {
            baseAttackSkill.StopSkillLogic();
            // baseActiveSkill.Initialize(_owner, baseActiveSkill.SkillData);
            baseAttackSkill.StartSkillLogicProcessAsync().Forget();
        }

        public void UseAllSkillList(bool useRepeatSkill, bool useSequenceSkill, CreatureController targetCreature)
        {
            if (useRepeatSkill)
            {
                var repeatSkillList = _activateAttackSkillList.FindAll(v => v is RepeatAttackSkill);
                repeatSkillList.ForEach(UseSkill);
            }

            if (useSequenceSkill)
            {
                UseSequenceSkill(targetCreature);
            }
        }

        private async void UseSequenceSkill(CreatureController targetCreature)
        {
            var sequenceSkillList = _activateAttackSkillList.FindAll(v => v is SequenceAttackSkill);
            if (sequenceSkillList.Count == 0)
            {
                return;
            }

            sequenceSkillList.ForEach(v =>
            {
                UpgradeOrAddAttackSkill(v.AttackSkillData);
                (v as SequenceAttackSkill).SetTargetCreature(targetCreature);
            });

            _useSequenceSkillCts = new CancellationTokenSource();

            try
            {
                //잠깐의 유예기간을 준다.
                await UniTask.WaitForSeconds(1f, cancellationToken: _useSequenceSkillCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSequenceSkill)} log : {e}");
                return;
            }

            int index = 0;
            while (_useSequenceSkillCts != null && !_useSequenceSkillCts.IsCancellationRequested)
            {
                var sequenceSkill = sequenceSkillList[index];
                try
                {
                    await sequenceSkill.StartSkillLogicProcessAsync();
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSequenceSkill)} log : {e}");
                    break;
                }

                index++;
                index %= sequenceSkillList.Count;

            }
        }

        public void StopSequenceSkill()
        {
            Utils.SafeCancelCancellationTokenSource(ref _useSequenceSkillCts);
        }

        public void StopAllSkillLogic()
        {
            _activateAttackSkillList.ForEach(v => v.StopSkillLogic());
            StopSequenceSkill();
        }

        public void AddActivateEquipmentSkillList(EquipmentSkillData equipmentSkillData)
        {
            if (equipmentSkillData.EquipmentSkillType == EquipmentSkillType.AuraDamage)
            {
                AuraSkill auraSkill = new AuraSkill();
                auraSkill.Initialize(_owner, equipmentSkillData);
                _activateEquipmentSkillList.Add(auraSkill);
            }
        }

        #endregion

        #region RecommendSkill

        //Passive + Skill
        /*
         *  레벨 1일 때에는 공격 스킬만 등장
         *  레벨 2부터는 공격 + 패시브 스킬 같이 등장
         *  스킬 등장은 가중치에 의해서 결정된다.
         *
         *  신규 스킬 -> 선택된 스킬이 6개 이하인 경우에만
         *  액티브 또는 패시브 스킬을 모두 습득했다면 -> 액티드 또는 패시브 스킬만 등장
         */
        public List<BaseSkillData> GetRecommendSkillList(int recommendSkillCount = 3)
        {
            List<BaseSkillData> pickedSkillList = new List<BaseSkillData>();
            PlayerModel model = ModelFactory.CreateOrGetModel<PlayerModel>();
            int currentSkillSelectCount = model.CurrentSkillSelectCount.Value;

            DataManager dataManager = Manager.I.Data;
            int onlyAttackSkillUpToCount =
                (int)dataManager.GlobalConfigDataDict[GlobalConfigName.OnlyAttackSkillUpToCount].Value;
            Dictionary<int, float> probabilityDict = new Dictionary<int, float>();

            for (int i = 0; i < recommendSkillCount; i++)
            {
                probabilityDict.Clear();
                float sumValue = 0;

                // --- 공격 스킬 로직 ---
                foreach (AttackSkillData skillData in AttackSkillDataList)
                {
                    if (pickedSkillList.Contains(skillData))
                    {
                        continue;
                    }

                    BaseAttackSkill skill =
                        _activateAttackSkillList.Find(v => v.AttackSkillType == skillData.AttackSkillType);
                    float value = 0;

                    // [수정] if-else if 구조로 변경하여 조건을 명확히 분리

                    // 1. 보유 중이고, 레벨업 가능한 경우
                    if (skill != null && skill.CurrentLevel < Const.MAX_AttackSKiLL_Level)
                    {
                        if (skill.CurrentLevel == Const.MAX_AttackSKiLL_Level - 1 &&
                            IsActivatedMatchPassiveSkill(skillData.MatchSkillId))
                        {
                            value = dataManager.GlobalConfigDataDict[GlobalConfigName.WeightOwnedSkill].Value;
                            probabilityDict[skillData.DataId] = value;
                        }
                        else if (skill.CurrentLevel < Const.MAX_AttackSKiLL_Level - 1)
                        {
                            value = dataManager.GlobalConfigDataDict[GlobalConfigName.WeightOwnedSkill].Value;
                            probabilityDict[skillData.DataId] = value;
                        }
                    }
                    // 2. 새로운 스킬이고, 배울 슬롯이 있는 경우
                    else if (skill == null && _activateAttackSkillList.Count < Const.MAX_SKILL_COUNT)
                    {
                        value = dataManager.GlobalConfigDataDict[GlobalConfigName.WeightNewSkill].Value;
                        probabilityDict[skillData.DataId] = value;
                    }

                    // (최대 레벨 스킬은 위 두 조건에 해당하지 않으므로 무시됨)

                    if (value != 0)
                    {
                        sumValue += value;
                    }
                }

                // --- 패시브 스킬 로직 ---
                if (currentSkillSelectCount > onlyAttackSkillUpToCount)
                {
                    foreach (PassiveSkillData skillData in PassiveSkillDataList)
                    {
                        if (pickedSkillList.Contains(skillData))
                        {
                            continue;
                        }

                        BasePassiveSkill skill =
                            _activatePassiveSkillList.Find(v => v.PassiveSkillType == skillData.PassiveSkillType);
                        float value = 0;

                        // [수정] if-else if 구조로 변경하여 조건을 명확히 분리

                        // 1. 보유 중이고, 레벨업 가능한 경우
                        if (skill != null && skill.CurrentLevel < Const.MAX_PassiveSkill_Level)
                        {
                            value = dataManager.GlobalConfigDataDict[GlobalConfigName.WeightOwnedSkill].Value;
                            probabilityDict[skillData.DataId] = value;
                        }
                        // 2. 새로운 스킬이고, 배울 슬롯이 있는 경우
                        else if (skill == null && _activatePassiveSkillList.Count < Const.MAX_SKILL_COUNT)
                        {
                            value = dataManager.GlobalConfigDataDict[GlobalConfigName.WeightNewSkill].Value;
                            probabilityDict[skillData.DataId] = value;
                        }

                        // (최대 레벨 스킬은 위 두 조건에 해당하지 않으므로 무시됨)

                        if (value != 0)
                        {
                            sumValue += value;
                        }
                    }
                }

                // 더 이상 픽할 수 있는 스킬이 없는 경우 루프 종료
                if (probabilityDict.Count == 0)
                {
                    break;
                }

                // --- 최종 스킬 선택 ---
                int pickSkillId = Utils.PickNumber(probabilityDict, sumValue);
                if (dataManager.AttackSkillDict.TryGetValue(pickSkillId, out AttackSkillData attackSkillData))
                {
                    BaseAttackSkill skill =
                        _activateAttackSkillList.Find(v => v.AttackSkillType == attackSkillData.AttackSkillType);
                    attackSkillData.CurrentLevel = skill == null ? 0 : skill.CurrentLevel;
                    pickedSkillList.Add(attackSkillData);
                }
                else if (dataManager.PassiveSkillDataDict.TryGetValue(pickSkillId,
                             out PassiveSkillData passiveSkillData))
                {
                    BasePassiveSkill skill =
                        _activatePassiveSkillList.Find(v => v.PassiveSkillType == passiveSkillData.PassiveSkillType);
                    passiveSkillData.CurrentLevel = skill == null ? 0 : skill.CurrentLevel;
                    pickedSkillList.Add(passiveSkillData);
                }
            }

            model.CurrentSkillSelectCount.Value++;
            return pickedSkillList;
        }

        private bool IsActivatedMatchPassiveSkill(int matchSKillId)
        {
            BasePassiveSkill skill = _activatePassiveSkillList.Find(v => v.SkillData.DataId == matchSKillId);
            return skill != null;
        }

        public bool IsActivateMatchAttackSKill(int matchSKillId)
        {
            foreach (BaseAttackSkill baseAttackSkill in _activateAttackSkillList)
            {
                int dataId = baseAttackSkill.SkillData.DataId;
                if (dataId >= matchSKillId && dataId <= matchSKillId + 5)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}