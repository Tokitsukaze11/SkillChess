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
    public AttackDecorator(Pawn pawn, int damage, int attackRange, AttackType attackType)
    {
        _curPawn = pawn;
        _damage = damage;
        _attackRange = attackRange;
        _attackType = attackType;
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
        targetSquares.Where(x => x.IsAnyPawn()).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
        {
            x.SetColor(GlobalValues.SELECABLE_COLOUR);
            x.OnClickSquare += SkillEffect;
        });
        _targetSquares.AddRange(targetSquares);
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        // TODO : Animation and Effect
        targetSquare.CurPawn?.TakeDamage(_damage);
        OnSkillEnd?.Invoke();
    }
}
