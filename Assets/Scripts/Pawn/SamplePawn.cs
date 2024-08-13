using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;

public class SamplePawn : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Straight;
        _isConsiderObstacle = true;
        _isHowitzerAttack = false;
        //_skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        _skill = new AttackDecorator(this,20,5,AttackType.ConsiderOtherPawnTarget, null);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            //OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        /*_skill = new HowitzerDecorator(this, 20, 5, 5);
        (_skill as HowitzerDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        /*_skill = new HealDecorator(this, 10, 5,HealType.Single);
        (_skill as HealDecorator)!.OnSkillEnd += () =>
        {
            _outlineFx.ToList().ForEach(x => x.enabled = false);
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        /*_skill = new DefendDecorator(this, 5);
        (_skill as DefendDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
    }
}
