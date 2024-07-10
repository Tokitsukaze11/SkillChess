using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering.Universal;

public class SamplePawn : Pawn
{
    private new void Awake()
    {
        base.Awake();
        //_skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        /*_skill = new AttackDecorator(this,20,5,AttackType.ConsiderOtherPawnTarget);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        /*_skill = new HowitzerDecorator(this, 20, 5, 5);
        (_skill as HowitzerDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        _skill = new HealDecorator(this, 10, 5,HealType.Single);
        (_skill as HealDecorator)!.OnSkillEnd += () =>
        {
            _outlineFx.enabled = false;
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        /*_skill = new DefendDecorator(this, 5);
        (_skill as DefendDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
    }
    protected override IEnumerator Co_EnemyMove()
    {
        yield return new WaitForSeconds(3); // TODO : AI
        // 임시로 3초 대기 후 턴 종료
        GameManager.Instance.TurnEnd();
        yield break;
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (_curHealth <= 0)
            Die();
    }
    protected override void Die()
    {
        Debug.Log($"{this.gameObject.name} is dead");
        OnDie?.Invoke(this);
    }
}
