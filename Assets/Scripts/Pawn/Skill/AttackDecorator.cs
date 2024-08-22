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
                _curPawn.ObjectTriggerAnimation.ResetTrigger();
            };
            Vector3 target = new Vector3(targetSquare.transform.position.x, 0, targetSquare.transform.position.z);
            _curPawn.transform.rotation = Quaternion.LookRotation(target - _curPawn.gameObject.transform.position);
            _curPawn.SkillAnimation();
            yield return new WaitForSeconds(3f);
            _curPawn.transform.rotation = Quaternion.LookRotation(GameManager.Instance.IsPlayer1Turn.Invoke() ? Vector3.forward : Vector3.back);
            OnSkillEnd?.Invoke();
            yield break;
        }
        var curSq = SquareCalculator.CurrentMapSquare(_curMapSquareIndex);
        var attackPath = MoveNavigation.FindNavigation(curSq, targetSquare);
        var realPath = new Queue<MapSquare>(attackPath.SkipLast(1));
        var reversPathQueue = new Queue<MapSquare>();
        for(int i = realPath.Count - 1; i >= 0; i--)
        {
            reversPathQueue.Enqueue(realPath.ElementAt(i));
        }
        _curPawn.ObjectTriggerAnimation.OnAnimationEndTrigger += () =>
        {
            _curPawn.CurMapSquare = realPath.Last();
            _curPawn.MoveOrder(reversPathQueue, () =>
            {
                _curPawn.CurMapSquare = curSq; // Fix Error but feel FUCK
                _curPawn.ObjectTriggerAnimation.ResetEndTrigger();
                _curPawn.transform.rotation = GameManager.Instance.IsPlayer1Turn.Invoke() ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
                OnSkillEnd?.Invoke();
            });
        };
        _curPawn.ObjectTriggerAnimation.OnAnimationTrigger += () =>
        {
            targetSquare.CurPawn?.TakeDamage(_damage, _hitParticleID);
            _curPawn.ObjectTriggerAnimation.ResetTrigger();
        };
        _curPawn.MoveOrder(realPath, () =>
        {
            Vector3 target = new Vector3(targetSquare.transform.position.x, 0, targetSquare.transform.position.z);
            _curPawn.transform.rotation = Quaternion.LookRotation(target - _curPawn.gameObject.transform.position);
            _curPawn.SkillAnimation();
        });
    }
}
