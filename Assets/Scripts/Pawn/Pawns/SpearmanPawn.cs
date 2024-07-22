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
        _skill = new HowitzerDecorator(this, 20, 5, 1,StringKeys.ATTACK_SKILL_HIT);
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
        var newSpear = ObjectManager.Instance.SpawnObject(_spear, null, false);
        newSpear.transform.position = _spear.transform.position;
        newSpear.transform.parent = _spear.transform.parent;
        _spear.SetActive(false);
        newSpear.SetActive(true);
        _objectTriggerAnimation.OnAnimationTrigger += () =>
        {
            newSpear.transform.parent = null;
            Destroy(newSpear,0.6f);
            newSpear.transform.DOMove(targetPos, 0.2f).onComplete += () =>
            {
                callback();
                newSpear.SetActive(false);
                _spear.SetActive(true);
            };
            _objectTriggerAnimation.ResetTrigger();
        };
        SkillAnimation();
        StartCoroutine(Co_SpearRotation(newSpear, targetPos));
    }
    private IEnumerator Co_SpearRotation(GameObject spear, Vector3 targetPos)
    {
        var spearMasterRotate = new Vector3(0, 90, 0);

        while (spear != null && spear.activeSelf)
        {
            Vector3 direction = targetPos - spear.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(spearMasterRotate);
            spear.transform.rotation = targetRotation;
            yield return null;
        }
        yield break;
    }
}
