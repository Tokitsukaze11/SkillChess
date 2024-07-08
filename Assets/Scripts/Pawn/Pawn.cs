using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public enum PawnType
{
    Pawn,
    King,
}

public abstract class Pawn : MonoBehaviour
{
    //protected List<Material> _materials = new List<Material>();
    public bool _isPlayerPawn;
    public bool _isCanClick = false;
    [SerializeField] protected int _health;
    [SerializeField] protected int _curHealth;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _defense;
    [SerializeField] protected int _shield = 0;
    protected int _curDefense = 0;
    [SerializeField] protected int _movementRange;
    [SerializeField] protected int _attackRange;
    [SerializeField] protected PawnType _pawnType;
    public DescriptObject[] _descriptObjects;
    [SerializeField] protected Transform _hpBarTransform;
    [SerializeField] protected SpriteRenderer _hpBar;
    [SerializeField] protected SpriteRenderer _hpBarRed;
    [SerializeField] protected SpriteRenderer _hpBarShield;
    public SortingGroup _sortingGroup;
    private Camera _mainCamera;
    protected MapSquare _curMapSquare;
    protected SkillDecorator _skill;
    public PawnType PawnType => _pawnType;
    public MapSquare CurMapSquare
    {
        set
        {
            _curMapSquare = value;
        }
        get
        {
            return _curMapSquare;
        }
    }
    public Action<bool,Pawn> OnPawnClicked;
    public Action<Pawn> OnDie;
    protected MapSquare _moveTargetSquare;
    protected void Awake()
    {
        _curHealth = _health;
        _hpBar.gameObject.transform.localScale = Vector3.one;
        _mainCamera = GameManager.Instance.mainCamera;
    }
    protected void Update()
    {
        var camRotation = _mainCamera.transform.rotation.eulerAngles;
        _hpBarTransform.rotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);
    }
    protected abstract void OnMouseDown();
    public virtual void ShowMoveRange()
    {
        
    }
    public abstract void Move();
    public abstract IEnumerator Co_Move(Queue<Vector2> path, Action callback);
    public abstract IEnumerator Co_EnemyMove();
    public virtual void ShowAttackRange()
    {
        
    }
    public abstract void Attack(Pawn targetPawn);
    public abstract void Defend();
    public abstract void UseSkill();
    public virtual void TakeDamage(int damage)
    {
        damage -= _curDefense;
        // TODO : Shield
        _curHealth -= damage;
        _curDefense = 0;
        var damParticle = ObjectManager.Instance.SpawnParticle(PawnManager.Instance._damageTextParticle, StringKeys.DAMAGE, true);
        var damageText = damParticle.GetComponent<DamageText>();
        var spawnPosition = this.transform.position;
        damageText.SetText(damage, spawnPosition, false);
        UpdateHpBar();
    }
    protected abstract void Die();
    private void UpdateHpBar()
    {
        var curHpBarX = _hpBar.transform.localScale.x;
        var targetHpBarX = (float)_curHealth / _health;
        var shieldBarX = Mathf.Clamp01((float)_shield / _health);
        _hpBarShield.transform.localScale = new Vector3(shieldBarX, 1, 1);
        if(curHpBarX > targetHpBarX) // 데미지를 받은 경우
        {
            _hpBar.transform.localScale = new Vector3(targetHpBarX, 1, 1);
            _hpBarRed.transform.DOScaleX(targetHpBarX, 0.5f).SetDelay(1f);
        }
        else // 힐을 받은 경우
        {
            _hpBar.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
            _hpBarRed.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
        }
        
        /*_hpBar.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
        _hpBarRed.transform.DOScaleX((float)_curHealth / _health, 0.5f).SetDelay(1f);*/
    }
    public void Heal(int healAmount)
    {
        _curHealth += healAmount;
        if (_curHealth > _health)
            _curHealth = _health;
        var healParticle = ObjectManager.Instance.SpawnParticle(PawnManager.Instance._damageTextParticle, StringKeys.DAMAGE, true);
        var healText = healParticle.GetComponent<DamageText>();
        healText.SetColour(Color.green);
        var spawnPosition = this.transform.position;
        healText.SetText(healAmount, spawnPosition);
        // TODO : Change text and animation for heal only
        Debug.Log("힐 전용 텍스트 및 애니메이션으로 변경 예정");
        UpdateHpBar();
    }
    public void GetShield(int shieldAmount)
    {
        _shield += shieldAmount;
        UpdateHpBar();
    }
}
