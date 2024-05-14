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
    public abstract void Move();
    public abstract void Attack();
    public abstract void Defend();
    public abstract void UseSkill();
    public abstract void TakeDamage(int damage);
    public abstract void Die();
}
