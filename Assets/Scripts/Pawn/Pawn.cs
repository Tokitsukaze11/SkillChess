using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum PawnType
{
    Pawn,
    Queen,
    King,
}

public abstract class Pawn : MonoBehaviour
{
    public bool _isPlayerPawn;
    public bool _isCanClick = false;
    [SerializeField] protected int _health;
    [SerializeField] protected int _curHealth;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _defense;
    protected int _curDefense = 0;
    [SerializeField] protected int _movementRange;
    [SerializeField] protected int _attackRange;
    [SerializeField] protected PawnType _pawnType;
    public DescriptObject[] _descriptObjects;
    [SerializeField] protected SpriteRenderer _hpBar;
    [SerializeField] protected SpriteRenderer _hpBarRed;
    public SortingGroup _sortingGroup;
    protected MapSquare _curMapSquare;
    public PawnType PawnType => _pawnType;
    public MapSquare CurMapSquare
    {
        set
        {
            _curMapSquare = value;
        }
    }
    public Action<bool,Pawn> OnPawnClicked;
    public Action<Pawn> OnDie;
    protected MapSquare _moveTargetSquare;
    protected void Awake()
    {
        _curHealth = _health;
        _hpBar.gameObject.transform.localScale = Vector3.one;
    }
    protected abstract void OnMouseDown();
    public virtual void ShowMoveRange()
    {
        
    }
    public abstract void Move();
    public abstract IEnumerator Co_EnemyMove();
    public virtual void ShowAttackRange()
    {
        
    }
    public abstract void Attack(Pawn targetPawn);
    public abstract void Defend();
    public abstract void UseSkill();
    public abstract void TakeDamage(int damage);
    protected abstract void Die();
}
