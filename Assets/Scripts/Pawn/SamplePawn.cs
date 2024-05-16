using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePawn : Pawn
{
    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        ShowMoveRange();
    }
    protected override void ShowMoveRange()
    {
        // TODO : Show move range
        GameManager.Instance.TurnEnd();
    }
    public override void Move()
    {
        if(!_isPlayerPawn)
            StartCoroutine(EnemyMove());
    }
    public override IEnumerator EnemyMove()
    {
        
        yield break;
    }
    public override void Attack()
    {
        
    }
    public override void Defend()
    {
        
    }
    public override void UseSkill()
    {
        
    }
    public override void TakeDamage(int damage)
    {
        
    }
    protected override void Die()
    {
        
    }
}
