using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpearmanPawn : Pawn
{
    [Header("SpearmanPawn")]
    [SerializeField] private GameObject _spear;
    
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Range;
        _isLessMove = true;
        _isConsiderObstacle = true;
        _isHowitzerAttack = false;
        _attackParticleID = StringKeys.NORMAL_ATTACK_HIT;
        _skillParticleID = StringKeys.ATTACK_SKILL_HIT;
        /*_skill = new AttackDecorator(this,20,5,AttackType.ConsiderOtherPawnTarget, _skillParticleID);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        _skill = new HowitzerDecorator(this, 20, 5, 1);
        (_skill as HowitzerDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        ((HowitzerDecorator)_skill).OnSkillAnimation += SkillAnim;
    }
    private void SkillAnim(Vector3 targetPos, Action callback)
    {
        _objectTriggerAnimation.OnAnimationTrigger += () =>
        {
            _spear.SetActive(false);
            var newSpear = ObjectManager.Instance.SpawnObject(_spear, null, false);
            newSpear.transform.position = _spear.transform.position;
            newSpear.transform.rotation = _spear.transform.rotation;
            newSpear.SetActive(true);
            newSpear.transform.DOMove(targetPos, 0.5f).OnComplete(() =>
            {
                callback();
                newSpear.SetActive(false);
                _spear.SetActive(true);
            });
            _objectTriggerAnimation.ResetTrigger();
        };
        SkillAnimation();
    }
}
