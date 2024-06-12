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
        var mapSquareDic = PawnManager.Instance.MapSquareDic;
        var keys = mapSquareDic.Keys.ToList(); // 64개의 키값을 리스트로 변환 8x8
        Vector2 nowKey = keys[_curMapSquareIndex];
        var curKeyIndex = keys.IndexOf(nowKey); // 현재 키값의 인덱스

        // Check target squares
        PawnManager.Instance.CheckTargetSquares(_attackRange, curKeyIndex, targetSquares);
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
        
        var mapSquareDic = PawnManager.Instance.MapSquareDic;
        var keys = mapSquareDic.Keys.ToList(); // 64개의 키값을 리스트로 변환 8x8
        var selectedKey = mapSquareDic.FirstOrDefault(x => x.Value == targetSquare[0]).Key;
        var curKeyIndex = keys.IndexOf(selectedKey); // 현재 키값의 인덱스
        
        var radial = new List<MapSquare>();
        
        // Check target squares
        // 상하좌우는 _areaRange만큼, 대각선은 _areaRange/2 만큼 단, 소숫점 이하는 버림
        PawnManager.Instance.CheckTargetSquares(_areaRange, curKeyIndex, radial);
        PawnManager.Instance.CheckDiagonalTargetSquares(_areaRange/2, curKeyIndex, radial);
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
