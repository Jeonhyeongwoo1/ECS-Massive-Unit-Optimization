using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MewVivor;
using MewVivor.Data;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Controller;
using MewVivor.InGame.Stat;
using MewVivor.InGame.Skill;
using MewVivor.Managers;
using MewVivor.Model;
using UniRx;
using UnityEngine;

public class CreatureController : MonoBehaviour, IHitable
{
    public string PrefabLabel=> _creatureData.PrefabLabel;
    public Vector3 Position => transform.position;
    public SkillBook SkillBook => _skillBook;
    public Rigidbody2D Rigidbody => _rigidbody;
    public CreatureData CreatureData => _creatureData;
    public bool IsDead => _creatureStateType == CreatureStateType.Dead;
    public CreatureType CreatureType => _creatureType;
    public CreatureStateType CreatureStateType => _creatureStateType;
    public Collider2D Collider => _collider ? _collider : null;
    public Vector3 CenterPosition => (Vector2)transform.position + _collider.offset;
    public virtual CreatureStat MaxHP { get; set; }
    public virtual CreatureStat Atk { get; set; }
    public virtual CreatureStat MoveSpeed { get; set; }
    public virtual CreatureStat CriticalPercent { get; set; }
    public virtual CreatureStat CriticalDamagePercent { get; set; }
    public virtual CreatureStat HpRecovery { get; set; }
    public ReactiveProperty<float> HP = new ReactiveProperty<float>();
    public Action<int, int> onHitReceived { get; set; }
    public Action<int, int> onHealReceived { get; set; }
    
    [SerializeField] protected CreatureStateType _creatureStateType;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Transform _pivotTransform;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Animator _animator;

    protected CreatureType _creatureType;
    protected SkillBook _skillBook;
    protected CreatureData _creatureData;
    protected Collider2D _collider;
    
    public virtual void Initialize(CreatureData creatureData, List<AttackSkillData> skillDataList)
    {
        // _spriteRenderer.sprite = sprite;
        _creatureData = creatureData;
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponentInChildren<Collider2D>();
        InitCreatureStat();
        AddSkillBook(skillDataList);
        Reset();
    }

    private void Reset()
    {
        _rigidbody.simulated = true;
        _creatureStateType = CreatureStateType.Idle;
    }

    /*
     * 총 공격력	: (기본 공격력 + 아웃게임 성장 공격력 + 장비 기본 스텟 합계 * (장비 공격력 증가율 + 투지 공격력 증가율)
     * 총 체력	: (기본 체력 + 아웃게임 성장 체력 + 장비 기본 스텟 합계 * 장비 체력 증가율 합산
     * 치명타 확률	: 기본 치명타 확률 + 성장 치명타 확률 + 장비 치명타 확률
     * 치명타 데미지	: 기본 치명타 데미지 증가율 + 성장 치명타 데미지 증가율 + 장비 치명타 데미지 증가율
     * 스킬 쿨타임 감소 :	기본 스킬 쿨타임 * (1- (성장 쿨타임 감소 수치율 + 장비 스킬 쿨타임 감소율))
     * 이동속도 :	기본 이동속도 * (성장 이동속도 증가율 + 장비 이동속도 증가율)
    */
    
    protected virtual void InitCreatureStat(bool isFullHp = true)
    {
    }

    private void AddSkillBook(List<AttackSkillData> skillDataList)
    {
        _skillBook ??= new SkillBook(this, skillDataList);
    }

    protected virtual void Dead()
    {
        HP.Value = 0;
        UpdateCreatureState(CreatureStateType.Dead);
    }

    public void Release()
    {
        Manager.I.Pool.ReleaseObject(gameObject.name, gameObject);
    }

    public virtual void TakeDamage(float damage, CreatureController attacker)
    {
        if (IsDead)
        {
            return;
        }
        
        HP.Value -= damage;
        if (HP.Value <= 0)
        {
            Dead();
        }
    }

    protected virtual async UniTask DeadAnimation()
    {
        ResourcesManager resource = Manager.I.Resource;
        // Material defaultMat =  resource.Load<Material>("CreatureDefaultMat");
        // Material hitEffectMat = resource.Load<Material>("PaintWhite");

        _rigidbody.simulated = false;
        // _spriteRenderer.material = defaultMat;
        await UniTask.WaitForSeconds(0.1f);
        
        // _spriteRenderer.material = hitEffectMat;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(0, 0.2f).SetEase(Ease.InOutBounce));
        sequence.OnComplete(() =>
        {
            _rigidbody.simulated = true;
            // _spriteRenderer.material = defaultMat;
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
        });

        try
        {
            await UniTask.WaitForSeconds(0.2f);
        }
        catch (Exception e) when(e is not OperationCanceledException)
        {
            Debug.LogError($"Error {e.Message}");
            return;
        }
    }

    private void SetSpriteFlipX(bool isRight)
    {
        _pivotTransform.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
    }

    public virtual Vector3 GetDirection()
    {
        return Vector3.zero;
    }
    
    protected virtual void Update(){}

    public virtual void UpdateStateAndAnimation(CreatureStateType stateType, string animationName)
    {
    }

    public void UpdateCreatureState(CreatureStateType stateType, bool updateAnimation = true)
    {
        _creatureStateType = stateType;

        switch (stateType)
        {
            case CreatureStateType.None:
                break;
            case CreatureStateType.Idle:
                break;
            case CreatureStateType.Move:
                break;
            case CreatureStateType.Skill:
                break;
            case CreatureStateType.Dead:
                break;
            case CreatureStateType.Stun:
                // _animator.SetBool();
                break;
            case CreatureStateType.Knockback:
                break;
        }
        
        if (updateAnimation)
        {
            UpdateStateAndAnimation(stateType, "");
        }
    }

    protected void MovePositionRigidBody(Vector2 direction)
    {
        if (_creatureStateType == CreatureStateType.Knockback 
                        || _creatureStateType == CreatureStateType.Stun
                        || _creatureStateType == CreatureStateType.None
                        || _creatureStateType == CreatureStateType.Dead)
        {
            return;
        }
        
        _rigidbody.MovePosition(_rigidbody.position + direction * (Time.fixedDeltaTime * MoveSpeed.Value));
        SetSpriteFlipX(direction.x > 0);
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        _skillBook?.StopAllSkillLogic();
    }
}
