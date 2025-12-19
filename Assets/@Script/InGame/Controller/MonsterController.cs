using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.InGame.Buff;
using MewVivor.InGame.Entity;
using MewVivor.InGame.Stat;
using MewVivor.Key;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MewVivor.InGame.Controller
{
    public class MonsterController : CreatureController
    {
        public MonsterType MonsterType => _monsterType;
        public int SpawnedWaveIndex => _spawnedWaveIndex;
        
        protected int _spawnedWaveIndex;
        protected MonsterType _monsterType;
        protected PlayerController _player;
        protected ActivateBuffState _activateBuffState;
        protected float _attackElapsedTime = 0;
        
        private CancellationTokenSource _takeDamageCts;
        private Sequence _takeDamageSequence;
        private StatModifer _moveSpeedModifer;
        private float _addDamagePercent;
        private Coroutine _knockbackCor = null;
        private GameObject _resourceObj;

        public override void Initialize(CreatureData creatureData, List<AttackSkillData> skillDataList)
        {
            if (_resourceObj == null)
            {
                _resourceObj = Manager.I.Resource.Instantiate(creatureData.ResourcePrefabLabel);
                _resourceObj.transform.SetParent(_pivotTransform);
                _resourceObj.transform.localPosition = Vector3.zero;
                _resourceObj.transform.localScale = Vector3.one * creatureData.Scale;
                _resourceObj.SetActive(true);
            }
            //Base에서 애니메이션 설정 후 처리
            _creatureType = CreatureType.Monster;

            if (_monsterType == MonsterType.None)
            {
                _monsterType = MonsterType.Normal;
            }

            base.Initialize(creatureData, skillDataList);
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _spriteRenderer.sortingOrder = Const.SortinglayerOrder_Monster;
            if (skillDataList != null && skillDataList.Count > 0)
            {
                _skillBook.UseAllSkillList(true, true, Manager.I.Object.Player);
            }
        }

        protected override void InitCreatureStat(bool isFullHp = true)
        {
            (float finalAtk, float finalHp) = Manager.I.Game.GetMonsterAtkAndHP(_monsterType, _creatureData);
            MaxHP = new CreatureStat(finalHp, CreatureStatType.MaxHP, this);
            Atk = new CreatureStat(finalAtk, CreatureStatType.Atk, this);
            MoveSpeed = new CreatureStat(CreatureData.MoveSpeed, CreatureStatType.MoveSpeed, this);
            HP.Value = MaxHP.Value;
        }

        private void Awake()
        {
            TryGetComponent(out _rigidbody);
        }

        protected override void OnEnable()
        {
            Manager.I.Event.AddEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            // Manager.I.Event.AddEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);
        }

        protected override void OnDisable()
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
            Manager.I.Event.RemoveEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            // Manager.I.Event.RemoveEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);

            AllCancelCancellationTokenSource();
            if (_resourceObj != null)
            {
                Manager.I.Pool.ReleaseObject(_resourceObj.name, _resourceObj);
                _resourceObj = null;
            }

            _knockbackCor = null;
            _elapsed = 0;
        }

        private void FixedUpdate()
        {
            if (Manager.I.Game.GameState == GameState.DeadPlayer)
            {
                return;
            }
            
            if (_knockbackCor != null)
            {
                return;
            }
            
            MovePositionRigidBody(GetDirection());
        }

        protected override void Update()
        {
            AttackProcess();
            BuffProcess();
            Lifecycle();
        }

        private float _elapsed = 0;
        private float _lifeTime = 20f;
        private void Lifecycle()
        {
            if (!Manager.I.Object.IsInCameraView(transform.position))
            {
                _elapsed += Time.deltaTime;
                if (_elapsed > _lifeTime)
                {
                    _elapsed = 0;
                    if (Manager.I.Game.GameType == GameType.INFINITY)
                    {
                        Manager.I.Object.ReleaseMonster(this);
                    }
                    else
                    {
                        int angle = Random.Range(0, 360);
                        float radius = Random.Range(30, 45);
                        Vector3 spawnPosition = Utils.GetCirclePosition(angle, radius);
                        transform.position = spawnPosition + _player.Position;
                    }
                }
            }
            else
            {
                _elapsed = 0;
            }
        }

        protected virtual void AttackProcess()
        {
            if (_player == null 
                || _player.IsDead 
                || _player.PlayerStateType == PlayerStateType.Invincibility
                || _creatureStateType == CreatureStateType.Dead)
            {
                return;
            }

            if (Utils.IsCollision(_collider, _player.Collider as CircleCollider2D))
            {
                _attackElapsedTime += Time.deltaTime;
                if (_attackElapsedTime > Const.MonsterAttackIntervalTime)
                {
                    _attackElapsedTime = 0;
                    // (기본 공격력 + ( StageLevel * 기본 공격력))
                    _player.TakeDamage(_creatureData.Atk, this);
                }
            }
        }
        
        private void BuffProcess()
        {
            for (int i = _activateBuffState.BuffCount - 1; i >= 0; i--)
            {
                BuffData buffData = _activateBuffState.BuffDataList[i];
                buffData.elapsed += Time.deltaTime;
                if (buffData.elapsed > buffData.duration)
                {
                    switch (buffData.debuffType)
                    {
                        case DebuffType.Stun:
                            UpdateCreatureState(CreatureStateType.Move);
                            break;
                        // case DebuffType.AddDamage:
                        //     _addDamagePercent = 0;
                        //     break;
                        case DebuffType.SlowSpeed:
                            if (_moveSpeedModifer != null)
                            {
                                MoveSpeed.RemoveModifer(_moveSpeedModifer);
                            }
                            break;
                    }

                    _activateBuffState.BuffDataList.RemoveAt(i);
                }
                else
                {
                    _activateBuffState.BuffDataList[i] = buffData;
                }
            }
        }

        public override Vector3 GetDirection()
        {
            return (_player.transform.position - transform.position).normalized;
        }

        public void ApplyDebuff(DebuffType debuffType, float debuffValue, float debuffValuePercent, float duration)
        {
            if (_activateBuffState.Equals(default))
            {
                _activateBuffState = new ActivateBuffState();
            }

            _activateBuffState.BuffDataList ??= new List<BuffData>();
            int index = _activateBuffState.BuffDataList.FindIndex(v => v.debuffType == debuffType);
            BuffData buffData;
            if (index >= 0)
            {
                buffData = _activateBuffState.BuffDataList[index]; // 복사본
            }
            else
            {
                buffData = new BuffData();
            }

            buffData.duration = duration;
            buffData.debuffType = debuffType;
            buffData.debuffValue = debuffValue;
            buffData.debuffValuePercent = debuffValuePercent;
            buffData.elapsed = 0;
            if (index >= 0)
            {
                _activateBuffState.BuffDataList[index] = buffData; // 다시 저장
            }
            else
            {
                switch (debuffType)
                {
                    case DebuffType.Stun:
                        if (_monsterType == MonsterType.Normal)
                        {
                            UpdateCreatureState(CreatureStateType.Stun);
                            _activateBuffState.BuffDataList.Add(buffData);
                        }
                        break;
                    case DebuffType.AddDamage:
                        _addDamagePercent = buffData.debuffValuePercent;
                        break;
                    case DebuffType.SlowSpeed:
                        if (_monsterType == MonsterType.Normal)
                        {
                            _moveSpeedModifer ??= new StatModifer(-debuffValue, ModifyType.PercentAdd, this);
                            MoveSpeed.AddModifier(_moveSpeedModifer);
                            _activateBuffState.BuffDataList.Add(buffData);
                        }
                        break;
                }
            }
        }

        public void RemoveBuff(DebuffType debuffType)
        {
            switch (debuffType)
            {
                case DebuffType.Stun:
                    UpdateCreatureState(CreatureStateType.Move);
                    break;
                case DebuffType.AddDamage:
                    _addDamagePercent = 0;
                    break;
                case DebuffType.SlowSpeed:
                    if (_moveSpeedModifer != null)
                    {
                        MoveSpeed.RemoveModifer(_moveSpeedModifer);
                    }
                    break;
            }
        }

        public void ExecuteKnockback(float knockbackDistance)
        {
            if (_knockbackCor != null || IsDead)
            {
                return;
            }

            if (_monsterType == MonsterType.Boss || _monsterType == MonsterType.Elite)
            {
                return;
            }

            UpdateCreatureState(CreatureStateType.Knockback);
            _knockbackCor = StartCoroutine(CorKnockback(knockbackDistance));
        }
        
        private IEnumerator CorKnockback(float knockbackDistance)
        {
            float elapsed = 0;
            while (true)
            {
                elapsed += Time.deltaTime;
                if (elapsed > Const.KNOCKBACK_TIME)
                {
                    break;
                }

                Vector3 direction = GetDirection() * (-1f * knockbackDistance);
                Vector2 nextVec = direction * (Const.KNOCKBACK_SPEED * Time.deltaTime);
                _rigidbody.MovePosition(_rigidbody.position + nextVec);
                yield return null;
            }

            yield return new WaitForSeconds(Const.KNOCKBACK_COOLTIME);

            _knockbackCor = null;
            if (IsDead)
            {
                yield break;
            }
            
            UpdateCreatureState(CreatureStateType.Move);
        }
        
        private void OnDeadPlayer(object value)
        {
            StopKnockback();
            AllCancelCancellationTokenSource();
        }

        private void StopKnockback()
        {
            if (_knockbackCor != null)
            {
                StopCoroutine(_knockbackCor);
                _knockbackCor = null;
            }
        }

        private void AllCancelCancellationTokenSource()
        {
        }

        public virtual void Spawn(Vector3 spawnPosition, PlayerController player, int spawnedWaveIndex)
        {
            transform.position = spawnPosition;
            UpdateCreatureState(CreatureStateType.Move);
            _player = player;
            gameObject.SetActive(true);
            _spawnedWaveIndex = spawnedWaveIndex;
            
            // _cellPos = Manager.I.Game.WorldToCell(spawnPosition);
            // FindPathToPlayer().Forget();
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            if (IsDead)
            {
                return;
            }

            bool isCritical = false;
            if (attacker is PlayerController player)
            {
                if (player != null)
                {
                    float ratio = Random.value;
                    if (ratio < player.CriticalPercent.Value)
                    {
                        damage *= player.CriticalDamagePercent.Value;
                        isCritical = true;
                    }
                }
            }

            if (_addDamagePercent > 0)
            {
                damage *= 1f + _addDamagePercent;
            }
            
            base.TakeDamage(damage, attacker);
            Manager.I.Object.ShowDamageFont(Position + new Vector3(0, 1.5f, 0),
                                        damage,
                                        0,
                                        transform,
                                        isCritical);

            if (Manager.I.Object.IsInCameraView(transform.position))
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.MonsterHit, 0.5f, 0.15f);
            }

            if (_takeDamageSequence != null)
            {
                _takeDamageSequence.Kill(true);
            }

            if (MonsterType == MonsterType.Normal || MonsterType == MonsterType.SuicideBomber)
            {
                Vector3 originScale = transform.localScale;
                _takeDamageSequence = DOTween.Sequence();
                _takeDamageSequence.Append(transform.DOScale(originScale * 0.8f, 0.15f))
                    // .SetEase(Ease.OutQuad)
                    .OnComplete(() => transform.localScale = originScale);
            }
        }

        protected override async void Dead()
        {
            if (IsDead)
            {
                return;
            }
            
            StopKnockback();
            base.Dead();
            await DeadAnimation();
            Manager.I.Object.DeadMonster(this);
            AllCancelCancellationTokenSource();
        }

        public void ForceKill()
        {
            Dead();
        }

        public virtual void TakeBombSkill()
        {
            ForceKill();
        }
        
        protected IEnumerator BossAndEliteSpawnAnimationCor(bool isBoss)
        {
            UpdateCreatureState(CreatureStateType.None);
            _pivotTransform.gameObject.SetActive(false);
            var prefab = Manager.I.Resource.Instantiate(nameof(BossAndEliteSpawnAlarmCircle));
            var circle = prefab.GetComponent<BossAndEliteSpawnAlarmCircle>();
            bool isCompleted = false;
            circle.Show(transform.position, () => isCompleted = true);

            if (isBoss)
            {
                Manager.I.Audio.Play(Sound.SFX, SoundKey.BossWarning).Forget();
                Manager.I.Audio.Play(Sound.BGM, SoundKey.BGM_BossWave).Forget();
            }
            
            yield return new WaitForSeconds(3f);
            
            _pivotTransform.gameObject.SetActive(true);
            circle.Hide();
            UpdateCreatureState(CreatureStateType.Move);
        }
        
        #region Cheat

        public void Cheat_ResizeScale(float scale)
        {
            if (_resourceObj)
            {
                _resourceObj.transform.localScale = Vector3.one * scale;
            }
        }

        #endregion
    }
}


        // private Vector3 _cellPos;
        //
        // private async UniTaskVoid FindPathToPlayer()
        // {
        //     _moveCts = new CancellationTokenSource();
        //     CancellationToken token = _moveCts.Token;
        //
        //     GameManager game = Manager.I.Game;
        //     float minSpeed = 1.5f;
        //     while (_moveCts != null && !_moveCts.IsCancellationRequested)
        //     {
        //         Vector3 myPos = transform.position;
        //         Vector3 playerPos = _player.Position;
        //         //일정거리 밑으로인경우에는 방향으로 밀기
        //         if ((myPos - playerPos).sqrMagnitude < 1.5f)
        //         {
        //             Vector3 dir = (playerPos - myPos).normalized;
        //             transform.position = Vector3.Lerp(myPos, myPos + dir, Time.deltaTime * minSpeed);
        //             SetSpriteFlipX(dir.x >= 0);
        //             await UniTask.Yield(cancellationToken: token);
        //             continue;
        //         }
        //
        //         // List<Vector3Int> list = new List<Vector3Int>();
        //         var list = game.PathFinding(game.WorldToCell(myPos), game.WorldToCell(playerPos));
        //         if (list == null || list.Count < 2)
        //         {
        //             Vector3 dir = (playerPos - myPos).normalized;
        //             transform.position = Vector3.Lerp(myPos, myPos + dir, Time.deltaTime * minSpeed);
        //         }
        //         else
        //         {
        //             var pathQueue = new Queue<Vector3Int>(list);
        //             // _pathQueue.Dequeue();
        //             while (pathQueue.Count > 0)
        //             {
        //                 var pos = pathQueue.Dequeue();
        //                 _cellPos = game.CellToWorld(pos);
        //
        //                 //이전과의 거리 차이가 많이 난다면 종료 후에 다시 길 찾기
        //                 if ((game.WorldToCell(playerPos) - game.WorldToCell(_player.transform.position)).magnitude > 1)
        //                 {
        //                     break;
        //                 }
        //             
        //                 while ((transform.position - _cellPos).sqrMagnitude >= 1f)
        //                 {
        //                     Vector3 dir = _cellPos - transform.position;
        //                     //일관성 있게 이동하기 위해서
        //                     float moveDist = Mathf.Min(dir.magnitude, MoveSpeed.Value * Time.deltaTime);
        //                     dir.Normalize();
        //                     transform.position += dir * moveDist;
        //                     SetSpriteFlipX(dir.x >= 0);
        //                     await UniTask.Yield();
        //                 }
        //             }
        //         }
        //
        //
        //         await UniTask.Yield();
        //     }
        // }
        //
        // private void OnResurrectionPlayer(object value)
        // {
        //     // FindPathToPlayer().Forget();
        // }
