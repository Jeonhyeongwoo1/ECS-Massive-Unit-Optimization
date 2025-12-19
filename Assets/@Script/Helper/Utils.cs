using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Stat;
using MewVivor.Key;
using MewVivor.Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MewVivor.Common
{
    public static class Utils
    {
        public static EquipmentGrade GetRandomEquipmentGrade(float[] prob)
        {
            float value = Random.value;
            if (value < prob[(int)EquipmentGrade.Epic])
            {
                return EquipmentGrade.Epic;
            }

            if (value < prob[(int)EquipmentGrade.Rare])
            {
                return EquipmentGrade.Rare;
            }

            if (value < prob[(int)EquipmentGrade.Uncommon])
            {
                return EquipmentGrade.Uncommon;
            }

            return EquipmentGrade.Common;
        }

        public static bool TrySpawnGem(ref GemType gemType, float purpleGemDropRate, float greenGemRatio,
            float blueGemRatio, float redGemDropRate)
        {
            float select = Random.value;
            if (select <= redGemDropRate)
            {
                gemType = GemType.RedGem;
                return true;
            }
            
            if (select <= purpleGemDropRate)
            {
                gemType = GemType.PurpleGem;
                return true;
            }

            if (select <= greenGemRatio)
            {
                gemType = GemType.GreenGem;
                return true;
            }
            
            gemType = GemType.BlueGem;
            return true;
        }

        public static T[] GetChildComponent<T>(Transform parent) where T  : Object
        {
            T[] childs = parent.GetComponentsInChildren<T>();
            if (childs.Length == 0)
            {
                return null;
            }

            T[] c = new T[childs.Length];
            for (var i = 0; i < childs.Length; i++)
            {
                if (parent == childs[i])
                {
                    continue;
                }

                c[i] = childs[i];
            }

            return c;
        }
        
        public static void SafeAddButtonListener(this Button button, UnityAction listener)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(()=> Manager.I.Audio.Play(Sound.SFX, SoundKey.Button_Click));
            button.onClick.AddListener(listener);
        }
        
        public static List<T> Shuffle<T>(this List<T> shuffleList)
        {
            int n = shuffleList.Count;
            while (n > 1)
            {
                n--;
                int random = Random.Range(0, n + 1);
                (shuffleList[random], shuffleList[n]) = (shuffleList[n], shuffleList[random]);
            }
            
            return shuffleList;
        }
        
        public static void SafeCancelCancellationTokenSource(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }
        
        public static Transform[] GetChilds(this Transform tr)
        {
            Transform[] children = new Transform[tr.transform.childCount];
            for (int i = 0; i < tr.transform.childCount; i++)
            {
                children[i] = tr.GetChild(i);
            }

            return children;
        }

        public static bool TryGetComponentInParent<T>(GameObject gameObject, out T t) where T : IHitable
        {
            if (gameObject.TryGetComponent<T>(out t))
            {
                return true;
            }

            var component = gameObject.GetComponentInParent<T>();
            if (component != null)
            {
                t = component;
                return true;
            }

            return false;
        }

        public static T AddOrGetComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }
        
        public static Vector3 GetPositionInDonut(Transform centerPosition, float minRange, float maxRange)
        {
            int angle = Random.Range(0, 360);
            float distX = Random.Range(minRange, maxRange);
            float distY = Random.Range(minRange, maxRange);
            float posX = Mathf.Cos(angle * Mathf.Deg2Rad) * distX;
            float posY = Mathf.Sign(angle * Mathf.Deg2Rad) * distY;
            Vector3 position = centerPosition.position + new Vector3(posX, posY);
            return position;
        }

        public static Color HexToColor(string color)
        {
            Color parsedColor;
            ColorUtility.TryParseHtmlString("#"+color, out parsedColor);

            return parsedColor;
        }

        public static TimeSpan GetOfflineRewardTime(DateTime lastOfflineGetRewardTime)
        {
            DateTime dateTime = lastOfflineGetRewardTime;
            TimeSpan timeSpan = DateTime.UtcNow - dateTime;
            if (timeSpan > TimeSpan.FromHours(24))
            {
                return TimeSpan.FromHours(23.9);
            }

            return timeSpan;
        }
        
        public static IPAddress GetIpv4Address(string hostAddress)
        {
            IPAddress[] ipAddr = Dns.GetHostAddresses(hostAddress);

            if (ipAddr.Length == 0)
            {
                return null;
            }
			
            foreach (IPAddress ipAddress in ipAddr)
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }
			
            return null;
        }

        public static float CalculateStatValue(float originValue, StatModifer statModifer)
        {
            if (statModifer == null)
            {
                return originValue;
            }
            
            float finalValue = originValue;
            float sumPercent = 0;
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
            
            finalValue *= (1 + sumPercent);
            return finalValue;
        }

        public static DropItemData GetDropItemControllerByItemBox()
        {
            List<DropItemData> dropItemDataList = Manager.I.Data.DropItemDict.Values.ToList();
            List<DropItemData> list = dropItemDataList.OrderByDescending(x => x.Value)
                .ToList();
            float random = Random.value;
            float cumulative = 0;
            DropItemData selectedDropItem = null;
            foreach (DropItemData dropItem in list)
            {
                cumulative += dropItem.PROB;
                if (random < cumulative)
                {
                    selectedDropItem = dropItem;
                    break;
                }
            }

            if (selectedDropItem == null)
            {
                Debug.LogError("Failed get selected Drop Item");
                return null;
            }

            return selectedDropItem;
        }

        public static int PickNumber(Dictionary<int, float> probabilityDict, float sumValue)
        {
            float random = Random.Range(0f, sumValue);
            float sum = 0;
            
            foreach (var (key, value) in probabilityDict)
            {
                sum += value;
                if (random < sum)
                {
                    return key;
                }
            }

            Debug.LogError("Failed pick number");
            return -1;
        }

        public static List<AttackSkillData> CreateDefaultSkillData()
        {
            List<AttackSkillData> list = new List<AttackSkillData>();
            DataManager dataManager = Manager.I.Data;
            foreach (AttackSkillType skill in System.Enum.GetValues(typeof(AttackSkillType)))
            {
                if (skill == AttackSkillType.None)
                {
                    continue;
                }
                
                int skillId = (int)skill;
                AttackSkillData skillData = dataManager.AttackSkillDict[skillId];
                list.Add(skillData);
            }

            return list;
        }
        
        public static List<PassiveSkillData> CreateDefaultPassiveSkillData()
        {
            List<PassiveSkillData> list = new List<PassiveSkillData>();
            DataManager dataManager = Manager.I.Data;
            foreach (PassiveSkillType skill in System.Enum.GetValues(typeof(PassiveSkillType)))
            {
                if (skill == PassiveSkillType.None)
                {
                    continue;
                }
                
                int skillId = (int)skill;
                PassiveSkillData passiveSkillData = dataManager.PassiveSkillDataDict[skillId];
                list.Add(passiveSkillData);
            }
            
            return list;
        }
        
        public static Vector3 GetCirclePosition(float angle, float radius)
        {
            // angle += Random.Range(0, 180);
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;
            return new Vector3(x, y);
        }

        public static Color GetSkillTypeColor(SkillType skillType, bool isActivate)
        {
            Color color;
            if (skillType == SkillType.AttackSkill)
            {
                color = isActivate ? Const.ActivateAttackSkillColor : Const.DeActivateAttackSkillColor;
            }
            else
            {
                color = isActivate ? Const.ActivatePassiveSkillColor : Const.DeActivatePassiveSkillColor;
            }

            return color;
        }
              
        public static bool IsCollision(Collider2D a, CircleCollider2D b)
        {
            Vector2 centerA = a.bounds.center;
            Vector2 centerB = b.transform.TransformPoint(b.offset);

            float approxRadiusA = Mathf.Max(a.bounds.extents.x, a.bounds.extents.y); // 가상의 반지름
            float radiusB = b.radius * Mathf.Max(b.transform.lossyScale.x, b.transform.lossyScale.y);

            float distanceSqr = (centerA - centerB).sqrMagnitude;
            float radius = approxRadiusA + radiusB;

            return distanceSqr <= radius * radius;
        }
        
        public static (int, float) GetCurrentLevelAndRemainExpRatio(long accumulatedExp)
        {
            float remainRatio = 0;
            int level = 1;
            List<AccountLevelData> list = Manager.I.Data.AccountLevelDict.Values.ToList();
            long exp = 0;
            for (var i = 0; i < list.Count; i++)
            {
                if (exp > accumulatedExp)
                {
                    level = i == 0 ? 1 : list[i - 1].Level;
                    remainRatio = (float)accumulatedExp / (float)exp;
                    break;
                }

                exp += list[i].NeedExp;
            }

            return (level, remainRatio);
        }
        
        public static string GetEquipmentGrade(EquipmentGrade equipmentGrade)
        {
            switch (equipmentGrade)
            {
                case EquipmentGrade.Common:
                    return Manager.I.Data.LocalizationDataDict["Grade_Common"].GetValueByLanguage();
                case EquipmentGrade.Uncommon:
                    return Manager.I.Data.LocalizationDataDict["Grade_Uncommon"].GetValueByLanguage();
                case EquipmentGrade.Rare:
                    return Manager.I.Data.LocalizationDataDict["Grade_Rare"].GetValueByLanguage();
                case EquipmentGrade.Epic:
                case EquipmentGrade.Epic1:
                case EquipmentGrade.Epic2:
                    return Manager.I.Data.LocalizationDataDict["Grade_Epic"].GetValueByLanguage();
                case EquipmentGrade.Legendary:
                case EquipmentGrade.Legendary1:
                case EquipmentGrade.Legendary2:
                case EquipmentGrade.Legendary3:
                    return Manager.I.Data.LocalizationDataDict["Grade_Legendary"].GetValueByLanguage();
                case EquipmentGrade.Myth:
                    return Manager.I.Data.LocalizationDataDict["Grade_Mythic"].GetValueByLanguage();
            }

            return "";
        }

        public static Sprite GetScrollSprite(EquipmentType equipmentType)
        {
            string name = null;
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    name = "Scroll_Weapon_Icon.sprite";
                    break;
                case EquipmentType.Gloves:
                    name = "Scroll_Gloves_Icon.sprite";
                    break;
                case EquipmentType.Ring:
                    name = "Scroll_Ring_Icon.sprite";
                    break;
                case EquipmentType.Belt:
                    name = "Scroll_Belt_Icon.sprite";
                    break;
                case EquipmentType.Armor:
                    name = "Scroll_Armor_Icon.sprite";
                    break;
                case EquipmentType.Boots:
                    name = "Scroll_Boots_Icon.sprite";
                    break;
            }

            Sprite sprite = Manager.I.Resource.Load<Sprite>(name);
            return sprite;
        }

        public static int GetGradeCount(EquipmentGrade grade)
        {
            switch (grade)
            {
                case EquipmentGrade.Common:
                case EquipmentGrade.Uncommon:
                case EquipmentGrade.Rare:
                case EquipmentGrade.Epic:
                case EquipmentGrade.Legendary:
                case EquipmentGrade.Myth:
                    return 0;
                case EquipmentGrade.Epic1:
                case EquipmentGrade.Legendary1:
                    return 1;
                case EquipmentGrade.Epic2:
                case EquipmentGrade.Legendary2:
                    return 2;
            }

            return 0;
        }

        public static float GetPlayerStat(CreatureStatType creatureStatType)
        {
            PlayerController player = Manager.I.Object.Player;
            if (player == null)
            {
                return 0;
            }
            
            switch (creatureStatType)
            {
                case CreatureStatType.MaxHP:
                    return player.MaxHP == null ? 1 : player.MaxHP.Value;
                case CreatureStatType.Atk:
                    return player.Atk == null ? 1 : player.Atk.Value;
                case CreatureStatType.MoveSpeed:
                    return player.MoveSpeed == null ? 1 : player.MoveSpeed.Value;
                case CreatureStatType.CriticalPercent:
                    return player.CriticalPercent == null ? 1 : player.CriticalPercent.Value;
                case CreatureStatType.HpRecovery:
                    return player.HpRecovery == null ? 0 : player.HpRecovery.Value;
                case CreatureStatType.CriticalDamagePercent:
                    return player.CriticalDamagePercent == null ? 1 : player.CriticalDamagePercent.Value;
                case CreatureStatType.Exp:
                    return player.ExpStat == null ? 1 : player.ExpStat.Value;
                case CreatureStatType.Defense:
                    return player.Defense == null ? 1 : player.Defense.Value;
                case CreatureStatType.AddGoldAmount:
                    return player.GoldAmountPercent == null ? 1 : player.GoldAmountPercent.Value;
                case CreatureStatType.MagneticRangePercent:
                    return player.MagneticRangePercent == null ? 1 : player.MagneticRangePercent.Value;
                case CreatureStatType.SkillCoolTime:
                    return player.SkillCoolTime == null ? 1 : player.SkillCoolTime.Value;
                case CreatureStatType.HpRecoveryEfficiency:
                    return player.HpRecoveryEfficiency == null ? 1 : player.HpRecoveryEfficiency.Value;
                case CreatureStatType.AuraDamageUpPercent:
                    return player.AuraDamageUpPercent == null ? 1 : player.AuraDamageUpPercent.Value;
                case CreatureStatType.ExplosionSkillSize:
                    return player.ExplosionSkillSize == null ? 1 : player.ExplosionSkillSize.Value;
                case CreatureStatType.BulletSkillSize:
                    return player.BulletSkillSize == null ? 1 : player.BulletSkillSize.Value;
                case CreatureStatType.BasicSkillCoolTime:
                    return player.BasicSkillCoolTime == null ? 0 : player.BasicSkillCoolTime.Value;
                case CreatureStatType.CircleSkillSize:
                    return player.CircleSkillSize == null ? 1 : player.CircleSkillSize.Value;
                case CreatureStatType.ItemBoxSpawnCoolTime:
                    return player.ItemBoxSpawnCoolTime == null ? 1 : player.ItemBoxSpawnCoolTime.Value;
            }

            Debug.LogError("failed get stat " + creatureStatType);
            return 0;
        }

        public static async UniTaskVoid DelayCallAsync(float delaySeconds, UnityAction action)
        {
            await UniTask.WaitForSeconds(delaySeconds);
            action?.Invoke();
        }

        public static IEnumerator DelayCall(float delayCall, UnityAction action)
        {
            yield return new WaitForSeconds(delayCall);
            action?.Invoke();
        }
        
        // public static Sprite GetEquipmentIcon(EquipmentType equipmentType)
        // {
        //     switch (equipmentType)
        //     {
        //         case EquipmentType.Weapon:
        //         case EquipmentType.Gloves:
        //         case EquipmentType.Ring:
        //             
        //             string iconSpriteName = $"{We}_Icon.sprite";
        //             var sprite = Manager.I.Resource.Load<Sprite>(iconSpriteName);
        //             return sprite;
        //         case EquipmentType.Belt:
        //         case EquipmentType.Armor:
        //         case EquipmentType.Boots:
        //             break;
        //     }
        //
        //     return null;
        // }
    }
}
