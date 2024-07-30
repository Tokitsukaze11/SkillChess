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
        if(targetSquares.Where(x => x.IsAnyPawn()).ToList().Where(x => x.CurPawn._isPlayerPawn).ToList().Count == 0)
        {
            _curPawn.CannotUseSkill();
            return;
        }
        targetSquares.Where(x => x.IsAnyPawn()).ToList().Where(x => x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
        {
            x.SetColor(GlobalValues.ATTACKABLE_COLOUR);
            x.OnClickSquare += SkillEffect;
        });
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        base.SkillEffect(targetSquare);
        
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
    // With Animation When Single Heal
    protected override IEnumerator Co_SkillEffect(MapSquare targetSquare)
    {
        yield return new WaitForSeconds(0.5f);
        _curPawn.ObjectTriggerAnimation.OnAnimationTrigger += () =>
        {
            var healParticle = ObjectManager.Instance.SpawnParticleViaID(StringKeys.HEAL_PARTICLE);
            Vector3 healPos = targetSquare.transform.position;
            healPos += new Vector3(0, 0.2f, 0);
            healParticle.transform.position = healPos;
            healParticle.SetActive(true);
            targetSquare.CurPawn?.Heal(_healAmount);
            OnSkillEnd?.Invoke();
            _curPawn.ObjectTriggerAnimation.ResetTrigger();
        };
        _curPawn.SkillAnimation();
        yield break;
    }
    // With Animation When Area Heal
    private IEnumerator Co_SkillEffectArea(List<MapSquare> mapSquares)
    {
        yield return new WaitForSeconds(0.5f);
        _curPawn.ObjectTriggerAnimation.OnAnimationTrigger += () =>
        {
            foreach(var square in mapSquares)
            {
                if (square.CurPawn == null)
                    continue;
                if(!square.CurPawn._isPlayerPawn)
                    continue;
                var healParticle = ObjectManager.Instance.SpawnParticleViaID(StringKeys.HEAL_PARTICLE);
                Vector3 healPos = square.transform.position;
                healPos += new Vector3(0, 0.2f, 0);
                healParticle.transform.position = healPos;
                healParticle.SetActive(true);
                square.CurPawn?.Heal(_healAmount);
            }
            OnSkillEnd?.Invoke();
            _curPawn.ObjectTriggerAnimation.ResetTrigger();
        };
        _curPawn.SkillAnimation();
        yield break;
    }
    // Without Animation
    private IEnumerator Co_SkillEffectNoAnim(List<MapSquare> mapSquares)
    {
        foreach(var square in mapSquares)
        {
            if (square.CurPawn == null)
                continue;
            if(!square.CurPawn._isPlayerPawn)
                continue;
            var healParticle = ObjectManager.Instance.SpawnParticleViaID(StringKeys.HEAL_PARTICLE);
            Vector3 healPos = square.transform.position;
            healPos += new Vector3(0, 0.2f, 0);
            healParticle.transform.position = healPos;
            healParticle.SetActive(true);
            square.CurPawn?.Heal(_healAmount);
        }
        OnSkillEnd?.Invoke();
        yield break;
    }
    private void HealSingle(MapSquare targetSquare, bool isTick = false)
    {
        CoroutineManager.Instance.AsyncStartViaCoroutine(!isTick ? Co_SkillEffect(targetSquare) : Co_SkillEffectNoAnim(new List<MapSquare>(){targetSquare}));
    }
    private void HealArea(MapSquare targetSquare, bool isTick = false)
    {
        var selectedIndex = SquareCalculator.CurrentIndex(targetSquare);
        
        // Check target squares
        // 상하좌우는 _healRange만큼, 대각선은 _healRange/2 만큼 단, 소숫점 이하는 버림
        var dir = new List<MapSquare>();
        SquareCalculator.CheckTargetSquares(_healRange, selectedIndex, dir);
        var area = new List<MapSquare>();
        SquareCalculator.CheckDiagonalTargetSquares(_healRange/2, selectedIndex, area);
        var radial = dir.Concat(area).ToList();
        radial.Add(targetSquare);
        CoroutineManager.Instance.AsyncStartViaCoroutine(!isTick ? Co_SkillEffectArea(radial) : Co_SkillEffectNoAnim(radial));
    }
    private void TickHeal(int healType, MapSquare targetSquare)
    {
        _curTick--;
        switch(healType)
        {
            case 4: // 지속 지정 힐
                HealSingle(targetSquare, true);
                break;
            case 8: // 지속 장판 힐
                HealArea(targetSquare, true);
                break;
        }
        if(_curTick == 0)
        {
            _curTick = _tickCount;
            PawnManager.Instance.OnPlayerTurn -= TickHealHandler;
        }
    }
}
