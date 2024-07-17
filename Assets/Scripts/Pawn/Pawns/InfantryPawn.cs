using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryPawn : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Straight;
        _isConsiderObstacle = true;
        _isHowitzerAttack = false;
        //_skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        _skill = new AttackDecorator(this,20,5,AttackType.ConsiderOtherPawnTarget);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
    }
}
