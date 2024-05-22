using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;

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
        _curHealth -= damage;
        var damageParticle = UIManager.Instance.ShowUIParticle();
        Vector3 pos = GameManager.Instance.mainCamera.GetComponent<Camera>().WorldToScreenPoint(this.transform.position);
        var rect = damageParticle.GetComponent<RectTransform>();
        rect.rect.Set(pos.x, pos.y, rect.rect.width, rect.rect.height);
        var vector3 = damageParticle.transform.position;
        vector3.z = 0;
        damageParticle.transform.position = vector3;
        damageParticle.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        /*float nowY = damageParticle.GetComponent<RectTransform>().anchoredPosition.y;
        damageParticle.GetComponent<RectTransform>().DOAnchorPosY(nowY+10, 1).OnComplete(() => ObjectManager.Instance.RemoveObject(damageParticle, StringKeys.DAMAGE, true));*/
        /*damageParticle.transform.position = this.transform.position;
        damageParticle.GetComponent<TextMeshPro>().text = damage.ToString();
        damageParticle.transform.DOMoveY(3, 1).OnComplete(() => ObjectManager.Instance.RemoveObject(damageParticle, StringKeys.DAMAGE, true));*/
        if (_curHealth <= 0)
            Die();
    }
    protected override void Die()
    {
        Debug.Log($"{this.gameObject.name} is dead");
        OnDie?.Invoke(this);
    }
}
