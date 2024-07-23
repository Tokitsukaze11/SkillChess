using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanArcher : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Range;
        _isConsiderObstacle = true;
        _isHowitzerAttack = true;
        _isLessMove = true;
        _attackParticleID = StringKeys.NORMAL_ATTACK_HIT;
        _skillParticleID = StringKeys.ATTACK_SKILL_HIT;
        _skill = new AttackDecorator(this, 50, 5, AttackType.AllPawnsInRange, _skillParticleID);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
    }
}
