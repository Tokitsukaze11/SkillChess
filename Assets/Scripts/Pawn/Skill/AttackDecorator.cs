using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AttackType
{
    ConsiderOtherPawnTarget,
    AllPawnsInRange,
}
public class AttackDecorator : SkillDecorator
{
    private int _damage;
    private int _attackRange;
    private AttackType _attackType;
    private List<MapSquare> _targetSquares = new List<MapSquare>();
    public event Action OnSkillEnd;
    public AttackDecorator(Pawn pawn, int damage, int attackRange, AttackType attackType, string skillParticleID)
    {
        _curPawn = pawn;
        _damage = damage;
        _attackRange = attackRange;
        _attackType = attackType;
        _hitParticleID = skillParticleID;
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        AttackPreview();
        // TODO : UI disable except cancel button
    }
    private void AttackPreview()
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();

        // Check now values
        var targetSquares = new List<MapSquare>();
        
        bool isConsideringAnyPawn = _attackType == AttackType.ConsiderOtherPawnTarget;

        // Check target squares
        SquareCalculator.CheckTargetSquares(_attackRange, _curMapSquareIndex, targetSquares, false, isConsideringAnyPawn);
        var rangeSquares = targetSquares;
        targetSquares = targetSquares.Where(x => x.IsAnyPawn()).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList();
        if (targetSquares.Count == 0)
        {
            _curPawn.CannotUseSkill();
            return;
        }
        rangeSquares = rangeSquares.Except(targetSquares).ToList();
        rangeSquares.ForEach(x => x.SetColor(GlobalValues.ATTACKABLE_COLOUR));
        targetSquares.ForEach(x =>
        {
            x.SetColor(GlobalValues.ATTACKABLE_COLOUR);
            x.OnClickSquare += SkillEffect;
        });
        //_targetSquares.AddRange(targetSquares);
        _targetSquares.Clear();
        foreach(var square in targetSquares)
        {
            _targetSquares.Add(square);
        }
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        base.SkillEffect(targetSquare);
        CoroutineManager.Instance.AsyncStartViaCoroutine(Co_SkillEffect(targetSquare));
    }
    protected override IEnumerator Co_SkillEffect(MapSquare targetSquare)
    {
        yield return new WaitForSeconds(0.5f);
        if (_attackType == AttackType.AllPawnsInRange)
        {
            _curPawn.ObjectTriggerAnimation.OnAnimationTrigger += () =>
            {
                targetSquare.CurPawn?.TakeDamage(_damage, _hitParticleID);
                OnSkillEnd?.Invoke();
                _curPawn.ObjectTriggerAnimation.ResetTrigger();
            };
            Vector3 target = new Vector3(targetSquare.transform.position.x, 0, targetSquare.transform.position.z);
            _curPawn.transform.rotation = Quaternion.LookRotation(target - _curPawn.gameObject.transform.position);
            _curPawn.SkillAnimation();
            yield return new WaitForSeconds(3f);
            _curPawn.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            yield break;
        }
        var curSq = SquareCalculator.CurrentMapSquare(_curMapSquareIndex);
        var path = MoveNavigation.FindNavigation(curSq, targetSquare).SkipLast(1).ToList();
        Queue<Vector2> keyPath = new Queue<Vector2>();
        foreach(var sq in path)
        {
            keyPath.Enqueue(SquareCalculator.CurrentKey(sq));
        }
        Queue<Vector2> reversPath = new Queue<Vector2>();
        foreach(var key in keyPath.Reverse())
        {
            reversPath.Enqueue(key);
        }
        _curPawn.ObjectTriggerAnimation.OnAnimationEndTrigger += () =>
        {
            _curPawn.MoveOrder(reversPath, () =>
            {
                _curPawn.ObjectTriggerAnimation.ResetEndTrigger();
                _curPawn.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            });
        };
        _curPawn.ObjectTriggerAnimation.OnAnimationTrigger += () =>
        {
            targetSquare.CurPawn?.TakeDamage(_damage, _hitParticleID);
            OnSkillEnd?.Invoke();
            _curPawn.ObjectTriggerAnimation.ResetTrigger();
        };
        _curPawn.MoveOrder(keyPath, () =>
        {
            Vector3 target = new Vector3(targetSquare.transform.position.x, 0, targetSquare.transform.position.z);
            _curPawn.transform.rotation = Quaternion.LookRotation(target - _curPawn.gameObject.transform.position);
            _curPawn.SkillAnimation();
        });
    }
}
