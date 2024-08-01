using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDecorator
{
    public Pawn _curPawn;
    protected int _curMapSquareIndex;
    protected string _hitParticleID;
    public event Action<bool,Pawn> OnSkillUsed;
    public virtual void Initialize(){}
    public void UpdateCurIndex(int index)
    {
        _curMapSquareIndex = index;
    }
    public abstract void UseSkill();
    protected abstract void SkillPreview();
    protected virtual void SkillEffect(MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        _curPawn.ResetOutline();
        OnSkillUsed!.Invoke(false, null);
    }
    protected abstract IEnumerator Co_SkillEffect(MapSquare targetSquare);
    protected List<MapSquare> DefaultSkillPreview(int range)
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();
        var targetSquares = new List<MapSquare>();
        SquareCalculator.CheckTargetSquares(range, _curMapSquareIndex, targetSquares);
        return targetSquares;
    }
}
