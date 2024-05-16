using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SamplePawn : Pawn
{
    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        ShowMoveRange();
    }
    protected override void ShowMoveRange()
    {
        PawnManager.Instance.ResetSquaresColor();
        var targetSquares = new List<MapSquare>();
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        var mapSquareDic = PawnManager.Instance.MapSquareDic;
        MapSquare nowSquare = PawnManager.Instance.GetCurrentMapSquare(currentPos);
        Vector2 nowKey = mapSquareDic.FirstOrDefault(x => x.Value == nowSquare).Key;
        var keys = mapSquareDic.Keys.ToList(); // 64개의 키값을 리스트로 변환 8x8
        var curKeyIndex = keys.IndexOf(nowKey); // 현재 키값의 인덱스
        PawnManager.Instance.CheckTargetSquares(_movementRange, curKeyIndex, targetSquares);
        targetSquares.Where(x => x.IsCanMove()).ToList().ForEach(x => x.SetColor(Color.yellow));
        //GameManager.Instance.TurnEnd();
    }
    public override void Move()
    {
        if(!_isPlayerPawn)
            StartCoroutine(EnemyMove());
    }
    public override IEnumerator EnemyMove()
    {
        yield return new WaitForSeconds(3); // TODO : AI
        // 임시로 3초 대기 후 턴 종료
        GameManager.Instance.TurnEnd();
        yield break;
    }
    public override void Attack()
    {
        
    }
    public override void Defend()
    {
        
    }
    public override void UseSkill()
    {
        
    }
    public override void TakeDamage(int damage)
    {
        
    }
    protected override void Die()
    {
        
    }
}
