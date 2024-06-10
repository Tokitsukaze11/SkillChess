using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AttackType
{
    SingleTarget,
    AreaTarget,
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
    public override void Initialize()
    {
        
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        switch (_attackType)
        {
            case AttackType.SingleTarget:
                SinglePreview();
                break;
            case AttackType.AreaTarget:
                AreaPreview();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        // TODO : UI disable except cancel button
    }
    private void SinglePreview()
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
        targetSquares.Where(x => !x.IsCanMove()).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
        {
            x.SetColor(Color.yellow);
            x.OnClickSquare += (mapSquare) =>
            {
                SkillEffect();
            };
        });
        _targetSquares.AddRange(targetSquares);
    }
    private void AreaPreview()
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
        targetSquares.Where(x => !x.IsCanMove()).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
        {
            x.SetColor(Color.yellow);
            x.OnClickSquare += (mapSquare) =>
            {
                SkillEffect();
            };
        });
        _targetSquares.AddRange(targetSquares);
    }
    protected override void SkillEffect()
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        // TODO : Animation
        
        foreach(var targetSquare in _targetSquares)
        {
            targetSquare.CurPawn.TakeDamage(_damage);
        }
        OnSkillEnd?.Invoke();
    }
}
