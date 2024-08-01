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
        _attackParticleID = StringKeys.NORMAL_ATTACK_HIT;
        _skillParticleID = StringKeys.ATTACK_SKILL_HIT;
        //_skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        _skill = new AttackDecorator(this,20,5,AttackType.ConsiderOtherPawnTarget, _skillParticleID);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
    }
}
