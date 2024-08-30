using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public class HowitzerDecorator : SkillDecorator
{
    private int _damage;
    private int _attackRange;
    private int _areaRange;
    private List<MapSquare> _targetSquares;
    private bool _isIgnorePlayerPawn = false;
    public event Action OnSkillEnd;
    public event Action<Vector3, Action> OnSkillAnimation;
    public HowitzerDecorator(Pawn pawn, int damage, int attackRange, int areaRange, string hitParticleID, bool isIgnorePlayerPawn = false)
    {
        _curPawn = pawn;
        _damage = damage;
        _attackRange = attackRange;
        _areaRange = areaRange;
        _hitParticleID = hitParticleID;
        _isIgnorePlayerPawn = isIgnorePlayerPawn;
    }
    public override void UseSkill()
    {
        SkillPreview();
    }
    protected override void SkillPreview()
    {
        var targetSquares = DefaultSkillPreview(_attackRange);
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
        _targetSquares = targetSquares;
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        base.SkillEffect(targetSquare);
        Observable.FromCoroutine(() => Co_SkillEffect(targetSquare)).Subscribe();
    }
    protected override IEnumerator Co_SkillEffect(MapSquare targetSquare)
    {
        yield return new WaitForSeconds(0.5f);
        
        var radial = new List<MapSquare>();
        var selectedIndex = SquareCalculator.CurrentIndex(targetSquare);
        
        // Check target squares
        // 상하좌우는 _areaRange만큼, 대각선은 _areaRange/2 만큼 단, 소숫점 이하는 버림
        SquareCalculator.CheckTargetSquares(_areaRange, selectedIndex, radial);
        SquareCalculator.CheckDiagonalTargetSquares(_areaRange/2, selectedIndex, radial);
        Action OnSkill = () =>
        {
            targetSquare.CurPawn?.TakeDamage(_damage, _hitParticleID);

            if (radial.Count > 0)
            {
                // 방사 피해는 50% 감소
                var pawnSquares = radial.Where(x => x.IsAnyPawn() && x.CurPawn != null).ToList();
                if (_isIgnorePlayerPawn)
                    pawnSquares.ForEach(x => x.CurPawn.TakeDamage(_damage / 2, _hitParticleID));
                else
                    pawnSquares.Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x => x.CurPawn.TakeDamage(_damage / 2, _hitParticleID));
            }
            OnSkillEnd?.Invoke();
        };
        OnSkillAnimation?.Invoke(targetSquare.transform.position, OnSkill);
        yield break;
    }
}
