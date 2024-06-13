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
    private List<MapSquare> _targetSquares = new List<MapSquare>();
    public event Action OnSkillEnd;
    public HowitzerDecorator(Pawn pawn, int damage, int attackRange, int areaRange)
    {
        _curPawn = pawn;
        _damage = damage;
        _attackRange = attackRange;
        _areaRange = areaRange;
    }
    public override void Initialize()
    {
        
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();
        
        // Check now values
        var targetSquares = new List<MapSquare>();

        // Check target squares
        SquareCalculator.CheckTargetSquares(_attackRange, _curMapSquareIndex, targetSquares);
        // 범위 안에 있는 모든 칸이 타겟임 (방사 피해기 때문)
        targetSquares.ForEach(x =>
        {
            x.SetColor(Color.yellow);
            x.OnClickSquare += (mapSquare) =>
            {
                SkillEffect(new List<MapSquare>(){mapSquare});
            };
        });
        _targetSquares.AddRange(targetSquares);
    }
    protected override void SkillEffect(List<MapSquare> targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        var radial = new List<MapSquare>();
        var selectedIndex = SquareCalculator.CurrentIndex(targetSquare[0]);
        
        // Check target squares
        // 상하좌우는 _areaRange만큼, 대각선은 _areaRange/2 만큼 단, 소숫점 이하는 버림
        SquareCalculator.CheckTargetSquares(_areaRange, selectedIndex, radial);
        SquareCalculator.CheckDiagonalTargetSquares(_areaRange/2, selectedIndex, radial);
        // TODO : Animation
        // TODO : Need to check ignore player pawn or not
        _targetSquares[0].CurPawn?.TakeDamage(_damage);
        // 방사 피해는 50% 감소
        foreach(var square in radial)
        {
            square.CurPawn?.TakeDamage(_damage/2);
        }
        OnSkillEnd?.Invoke();
    }
}
