using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HowitzerDecorator : SkillDecorator
{
    private int _damage;
    private int _attackRange;
    private int _areaRange;
    private List<MapSquare> _targetSquares;
    private bool _isIgnorePlayerPawn = false;
    public event Action OnSkillEnd;
    public HowitzerDecorator(Pawn pawn, int damage, int attackRange, int areaRange, bool isIgnorePlayerPawn = false)
    {
        _curPawn = pawn;
        _damage = damage;
        _attackRange = attackRange;
        _areaRange = areaRange;
        _isIgnorePlayerPawn = isIgnorePlayerPawn;
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        var targetSquares = DefaultSkillPreview(_attackRange);
        targetSquares.Where(x => x.IsAnyPawn()).ToList().ForEach(x =>
        {
            x.SetColor(GlobalValues.SELECABLE_COLOUR);
            x.OnClickSquare += SkillEffect;
        });
        _targetSquares = targetSquares;
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        var radial = new List<MapSquare>();
        var selectedIndex = SquareCalculator.CurrentIndex(targetSquare);
        
        // Check target squares
        // 상하좌우는 _areaRange만큼, 대각선은 _areaRange/2 만큼 단, 소숫점 이하는 버림
        SquareCalculator.CheckTargetSquares(_areaRange, selectedIndex, radial);
        SquareCalculator.CheckDiagonalTargetSquares(_areaRange/2, selectedIndex, radial);
        // TODO : Animation
        targetSquare.CurPawn?.TakeDamage(_damage);
        //_targetSquares[0].CurPawn?.TakeDamage(_damage);
        // 방사 피해는 50% 감소
        var pawnSquares = radial.Where(x => x.IsAnyPawn() && x.CurPawn != null).ToList();
        if (_isIgnorePlayerPawn)
            pawnSquares.ForEach(x => x.CurPawn.TakeDamage(_damage / 2));
        else
            pawnSquares.Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x => x.CurPawn.TakeDamage(_damage / 2));
        OnSkillEnd?.Invoke();
    }
}
