using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDecorator
{
    public Pawn _curPawn;
    protected int _curMapSquareIndex;
    public abstract void Initialize();
    public virtual void UpdateCurIndex(int index)
    {
        _curMapSquareIndex = index;
    }
    public abstract void UseSkill();
    protected abstract void SkillPreview();
    protected abstract void SkillEffect(List<MapSquare> targetSquare);
}
