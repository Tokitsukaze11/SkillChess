using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHeal : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Straight;
        _isConsiderObstacle = true;
        _isHowitzerAttack = false;
        _attackParticleID = StringKeys.NORMAL_ATTACK_HIT;
        _skillParticleID = StringKeys.MAGIC_MINE;
        string skillOtherPawn = StringKeys.HEAL_PARTICLE;
        _skill = new HealDecorator(this, 20, 5, 2, HealType.Area);
        (_skill as HealDecorator)!.OnSkillEnd += () =>
        {
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
    }
}
