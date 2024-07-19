using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendDecorator : SkillDecorator
{
    private int _defendAmount;
    private int _defendRange;
    private string _mineParticleID;
    private string _skillParticleID;
    public event Action OnSkillEnd;
    public DefendDecorator(Pawn pawn, int defendAmount, string skillParticleID, string skillParticleMineID, int defendRange = 0)
    {
        _curPawn = pawn;
        _defendAmount = defendAmount;
        _defendRange = defendRange;
        _skillParticleID = skillParticleID;
        _mineParticleID = skillParticleMineID;
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
                x.SetColor(GlobalValues.SELECABLE_COLOUR);
                x.OnClickSquare += SkillEffect;
            });
        }
        else
        {
            var playerPawns = PawnManager.Instance.GetPawns(true);
            playerPawns.Select(x => x?.CurMapSquare).ToList().ForEach(x =>
            {
                x.SetColor(GlobalValues.SELECABLE_COLOUR);
                x.OnClickSquare += SkillEffect;
            });
        }
    }
    protected override void SkillEffect(MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        CoroutineManager.Instance.AsyncStartViaCoroutine(Co_SkillEffect(targetSquare));
    }
    protected override IEnumerator Co_SkillEffect(MapSquare targetSquare)
    {
        yield return new WaitForSeconds(0.5f);
        var otherParticle = ObjectManager.Instance.SpawnParticleViaID(_skillParticleID);
        Vector3 otherPos = targetSquare.transform.position;
        otherPos += new Vector3(0, 0.2f, 0);
        otherParticle.transform.position = otherPos;
        _curPawn.ObjectTriggerAnimation.OnAnimationTrigger += () =>
        {
            otherParticle.SetActive(true);
            targetSquare.CurPawn?.GetShield(_defendAmount);
            OnSkillEnd?.Invoke();
            _curPawn.ObjectTriggerAnimation.ResetTrigger();
        };
        var mineParticle = ObjectManager.Instance.SpawnParticleViaID(_mineParticleID);
        Vector3 targetPos = _curPawn.transform.position;
        targetPos += new Vector3(0, 0.2f, 0);
        mineParticle.transform.position = targetPos;
        mineParticle.SetActive(true);
        _curPawn.SkillAnimation();
        yield break;
    }
}
