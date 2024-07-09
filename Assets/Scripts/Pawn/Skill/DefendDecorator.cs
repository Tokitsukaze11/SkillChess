using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendDecorator : SkillDecorator
{
    private int _defendAmount;
    private int _defendRange;
    public event Action OnSkillEnd;
    public DefendDecorator(Pawn pawn, int defendAmount, int defendRange = 0)
    {
        _curPawn = pawn;
        _defendAmount = defendAmount;
        _defendRange = defendRange;
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        bool isRange = _defendRange > 0;
        if (isRange)
        {
            var targetSquares = DefaultSkillPreview(_defendRange);
            targetSquares.Where(x => x.IsAnyPawn()).ToList().Where(x => x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
            {
                x.SetColor(Color.yellow);
                x.OnClickSquare += SkillEffect;
            });
        }
        else
        {
            var playerPawns = PawnManager.Instance.GetPawns(true);
            playerPawns.Select(x => x?.CurMapSquare).ToList().ForEach(x =>
            {
                x.SetColor(Color.yellow);
                x.OnClickSquare += SkillEffect;
            });
        }
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        targetSquare.CurPawn?.GetShield(_defendAmount);
        OnSkillEnd?.Invoke();
    }
}
