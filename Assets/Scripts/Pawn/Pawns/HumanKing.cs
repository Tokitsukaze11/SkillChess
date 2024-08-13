using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanKing : Pawn
{
    [SerializeField] private AudioClip[] _attackSounds;
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Straight;
        _isConsiderObstacle = true;
        _isHowitzerAttack = false;
        _attackParticleID = StringKeys.NORMAL_ATTACK_HIT;
        _skillParticleID = StringKeys.MAGIC_MINE;
        string skillOtherPawn = StringKeys.MAGIC_OTHER;
        _hitSound = _attackSounds;
        _skill = new DefendDecorator(this, 10, skillOtherPawn, _skillParticleID);
        (_skill as DefendDecorator)!.OnSkillEnd += () =>
        {
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
    }
}
