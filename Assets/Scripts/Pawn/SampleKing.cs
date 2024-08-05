using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleKing : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        _moveType = MoveType.Straight;
        _isConsiderObstacle = true;
        _isHowitzerAttack = false;
        _pawnType = PawnType.King;
    }
}
