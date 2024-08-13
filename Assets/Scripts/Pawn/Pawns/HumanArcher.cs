using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanArcher : Pawn
{
    [SerializeField] private AudioClip[] _attackSound;
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Range;
        _isConsiderObstacle = true;
        _isHowitzerAttack = true;
        _isLessMove = true;
        _attackParticleID = StringKeys.NORMAL_ATTACK_HIT;
        _skillParticleID = StringKeys.ATTACK_SKILL_HIT;
        _hitSound = _attackSound;
        _skill = new AttackDecorator(this, 80, 5, AttackType.AllPawnsInRange, _skillParticleID);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            SoundManager.Instance.PlaySfx(_attackSound[Random.Range(0, _attackSound.Length)]);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
    }
}
