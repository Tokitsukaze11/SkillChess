using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    public bool _isPlayerPawn;
    [SerializeField] protected int _health;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _defense;
    [SerializeField] protected int _movementRange;
    protected MapSquare _curMapSquare;
    public MapSquare CurMapSquare
    {
        set
        {
            _curMapSquare = value;
        }
    }
    protected MapSquare _moveTargetSquare;
    protected abstract void OnMouseUp();
    protected virtual void ShowMoveRange()
    {
        
    }
    public abstract void Move();
    public abstract IEnumerator Co_EnemyMove();
    public abstract void Attack();
    public abstract void Defend();
    public abstract void UseSkill();
    public abstract void TakeDamage(int damage);
    protected abstract void Die();
}
