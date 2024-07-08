using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealType
{
    Single,
    Area,
}

public class HealDecorator : SkillDecorator
{
    private int _healAmount;
    private int _healRange;
    private HealType _healType;
    private int _tickCount;
    private int _curTick = 0;
    private bool _isTickHeal = false;
    private List<MapSquare> _targetSquares = new List<MapSquare>();
    private event Action TickHealHandler;
    public event Action OnSkillEnd;
    public HealDecorator(Pawn pawn, int healAmount, int healRange, HealType healType, int tickCount = 0)
    {
        _curPawn = pawn;
        _healAmount = healAmount;
        _healRange = healRange; 
        _healType = healType;
        _tickCount = tickCount;
        _curTick = _tickCount;
        _isTickHeal = _tickCount > 0;
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        var targetSquares = DefaultSkillPreview(_healRange);
        targetSquares.Where(x => x.IsAnyPawn()).ToList().Where(x => x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
        {
            x.SetColor(Color.yellow);
            x.OnClickSquare += SkillEffect;
        });
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        int tickHeal = _isTickHeal ? 1 << 3 : 1;
        int healType = tickHeal * (1 << (int)_healType);
        switch(healType)
        {
            case 1: // 지정 힐
                HealSingle(targetSquare);
                break;
            case 2: // 장판 힐
                HealArea(targetSquare);
                break;
            case > 3: // 지속 힐
                TickHealHandler += () => TickHeal(healType, targetSquare);
                PawnManager.Instance.OnPlayerTurn += TickHealHandler;
                break;
        }
    }
    private void HealSingle(MapSquare targetSquare)
    {
        targetSquare.CurPawn?.Heal(_healAmount);
        OnSkillEnd?.Invoke();
    }
    private void HealArea(MapSquare targetSquare)
    {
        var radial = new List<MapSquare>();
        var selectedIndex = SquareCalculator.CurrentIndex(targetSquare);
        
        // Check target squares
        // 상하좌우는 _healRange만큼, 대각선은 _healRange/2 만큼 단, 소숫점 이하는 버림
        SquareCalculator.CheckTargetSquares(_healRange, selectedIndex, radial);
        SquareCalculator.CheckDiagonalTargetSquares(_healRange/2, selectedIndex, radial);
        targetSquare.CurPawn?.Heal(_healAmount);
        foreach(var square in radial.Where(x => x.IsAnyPawn() && x.CurPawn._isPlayerPawn))
        {
            square.CurPawn.Heal(_healAmount/2);
        }
        OnSkillEnd?.Invoke();
    }
    private void TickHeal(int healType, MapSquare targetSquare)
    {
        _curTick--;
        switch(healType)
        {
            case 4: // 지속 지정 힐
                HealSingle(targetSquare);
                break;
            case 8: // 지속 장판 힐
                HealArea(targetSquare);
                break;
        }
        if(_curTick == 0)
        {
            _curTick = _tickCount;
            PawnManager.Instance.OnPlayerTurn -= TickHealHandler;
        }
    }
}
