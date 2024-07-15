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
    protected override IEnumerator Co_EnemyMove()
    {
        yield return new WaitForSeconds(3); // TODO : AI
        // 임시로 3초 대기 후 턴 종료
        GameManager.Instance.TurnEnd();
        yield break;
    }
}
