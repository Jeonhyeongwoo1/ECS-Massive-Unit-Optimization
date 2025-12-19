using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MewVivor.Common;
using MewVivor.Enum;
using MewVivor.InGame.Controller;
using MewVivor.Key;
using UnityEngine;

namespace MewVivor.InGame.Skill
{
    public class Skill_10121 : RepeatAttackSkill
    {
        private List<IGeneratable> _generatableList = new();
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            Release();
        }

        protected override async UniTask UseSkill()
        {
            _generatableList.Clear();
            int cnt = AttackSkillData.NumOfProjectile;
            List<Transform> monsterList = Manager.I.Object.GetCenterMonsterInCameraArea(cnt);
            if (monsterList == null)
            {
                for (int i = 0; i < cnt; i++)
                {
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10121, 0.5f, 0.25f);
                    GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    Vector3 position = Manager.I.Object.GetRandomPositionInCameraArea();
                    position.z = 0;
                    generatable.Generate(position, Vector3.zero, AttackSkillData, _owner);   
                    _generatableList.Add(generatable);
                }
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    if (monsterList.Count <= i)
                    {
                        break;
                    }
                
                    Manager.I.Audio.Play(Sound.SFX, SoundKey.UseSkill_10121, 0.5f, 0.25f);
                    GameObject prefab = Manager.I.Resource.Instantiate(AttackSkillData.PrefabLabel);
                    var generatable = prefab.GetComponent<IGeneratable>();
                    generatable.OnHit = OnHit;
                    generatable.Generate(monsterList[i].position, Vector3.zero, AttackSkillData, _owner);   
                    _generatableList.Add(generatable);
                }
            }
            
            try
            {
                //이펙트 대기시간
                await UniTask.WaitForSeconds(1, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                StopSkillLogic();
            }

            if (!IsMaxLevel)
            {
                _generatableList?.ForEach(v=> v.Release());
            }
        }
    }
}