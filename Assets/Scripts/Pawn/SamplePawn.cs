using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SamplePawn : Pawn
{
    protected override void OnMouseDown()
    {
        if (!_isPlayerPawn || !GameManager.Instance.IsPlayerTurn.Invoke())
            return;
        OnPawnClicked?.Invoke(true, this);
    }
    public override void ShowMoveRange()
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();
        // Check now values
        var targetSquares = new List<MapSquare>();
        var mapSquareDic = PawnManager.Instance.MapSquareDic;
        Vector2 nowKey = mapSquareDic.FirstOrDefault(x => x.Value == _curMapSquare).Key;
        var keys = mapSquareDic.Keys.ToList(); // 64개의 키값을 리스트로 변환 8x8
        var curKeyIndex = keys.IndexOf(nowKey); // 현재 키값의 인덱스
        // Check target squares
        PawnManager.Instance.CheckTargetSquares(_movementRange, curKeyIndex, targetSquares);
        targetSquares.Where(x => x.IsCanMove()).ToList().ForEach(x =>
        {
            x.SetColor(Color.yellow);
            x.OnClickSquare += (mapSquare) =>
            {
                _moveTargetSquare = mapSquare;
                Move();
            };
        });
    }
    public override void Move()
    {
        if(!_isPlayerPawn)
            StartCoroutine(Co_EnemyMove());
        
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        this.transform.position = new Vector3(_moveTargetSquare.transform.position.x, 1, _moveTargetSquare.transform.position.z);
        // 위 코드는 애니메이션으로 대체되어야 함
        
        // 아래 코드는 이동이 끝나면 실행되어야 함 (일단 지금은 기능만 구현)
        _curMapSquare.CurPawn = null;
        _curMapSquare = _moveTargetSquare;
        _moveTargetSquare.CurPawn = this;
        OnPawnClicked?.Invoke(false,null);
        GameManager.Instance.TurnEnd();
    }
    public override IEnumerator Co_EnemyMove()
    {
        yield return new WaitForSeconds(3); // TODO : AI
        // 임시로 3초 대기 후 턴 종료
        GameManager.Instance.TurnEnd();
        yield break;
    }
    public override void ShowAttackRange()
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();

        // Check now values
        var targetSquares = new List<MapSquare>();
        var mapSquareDic = PawnManager.Instance.MapSquareDic;
        Vector2 nowKey = mapSquareDic.FirstOrDefault(x => x.Value == _curMapSquare).Key;
        var keys = mapSquareDic.Keys.ToList(); // 64개의 키값을 리스트로 변환 8x8
        var curKeyIndex = keys.IndexOf(nowKey); // 현재 키값의 인덱스

        // Check target squares
        PawnManager.Instance.CheckTargetSquares(_attackRange, curKeyIndex, targetSquares);
        targetSquares.Where(x => !x.IsCanMove()).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
        {
            x.SetColor(Color.yellow);
            x.OnClickSquare += (mapSquare) =>
            {
                Attack(x.CurPawn);
            };
        });
    }
    public override void Attack(Pawn targetPawn)
    {
        if(!_isPlayerPawn)
            StartCoroutine(Co_EnemyMove());
        
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화

        targetPawn.TakeDamage(_damage);
        OnPawnClicked?.Invoke(false, null);
        GameManager.Instance.TurnEnd();
    }
    public override void Defend()
    {
        
    }
    public override void UseSkill()
    {
        
    }
    public override void TakeDamage(int damage)
    {
        Debug.Log($"{this.gameObject.name} takes {damage} damage");
    }
    protected override void Die()
    {
        
    }
}
