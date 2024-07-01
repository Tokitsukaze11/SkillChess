using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;

public class SamplePawn : Pawn
{
    private new void Awake()
    {
        base.Awake();
        _skill = new AttackDecorator(this,20,5,AttackType.SelectAttackTarget);
        //_skill = new HowitzerDecorator(this, 20, 5, 5);
        //_skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        /*(_skill as HowitzerDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
    }
    protected override void OnMouseDown()
    {
        if(_curMapSquare.IsCanClick()) // MapSquare을 누를 수 있게 보정
        {
            _curMapSquare.OnMouseDown();
            return;
        }
        if (!_isPlayerPawn || !_isCanClick)
            return;
        OnPawnClicked?.Invoke(true, this);
        PawnManager.Instance.ResetSquaresColor();
    }
    public override void ShowMoveRange()
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();
        // Check now values
        var targetSquares = new List<MapSquare>();
        /*var mapSquareDic = PawnManager.Instance.MapSquareDic;
        Vector2 nowKey = mapSquareDic.FirstOrDefault(x => x.Value == _curMapSquare).Key;
        var keys = mapSquareDic.Keys.ToList(); // 64개의 키값을 리스트로 변환 8x8
        var curKeyIndex = keys.IndexOf(nowKey); // 현재 키값의 인덱스*/
        int curKeyIndex = SquareCalculator.CurrentIndex(_curMapSquare);
        // Check target squares
        SquareCalculator.CheckTargetSquares(_movementRange, curKeyIndex, targetSquares,true);
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
        
        Vector2 curKey = SquareCalculator.CurrentKey(_moveTargetSquare);
        MoveNavigation.FindNavigation(_curMapSquare, _moveTargetSquare);
        Debug.Log("BFS Test");
        return;
        this.transform.position = new Vector3(curKey.x, 1, curKey.y);
        // 위 코드는 애니메이션으로 대체되어야 함
        
        // 아래 코드는 이동이 끝나면 실행되어야 함 (일단 지금은 기능만 구현)
        _curMapSquare.CurPawn = null;
        _curMapSquare = _moveTargetSquare;
        _moveTargetSquare.CurPawn = this;
        int curKeyIndexInt = SquareCalculator.CurrentIndex(_curMapSquare);
        _skill.UpdateCurIndex(curKeyIndexInt);
        OnPawnClicked?.Invoke(false,null);
        _curDefense = 0;
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
        var curKeyIndex = SquareCalculator.CurrentIndex(_curMapSquare);

        // Check target squares
        SquareCalculator.CheckTargetSquares(_attackRange, curKeyIndex, targetSquares);
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
        _curDefense = 0;
        GameManager.Instance.TurnEnd();
    }
    public override void Defend()
    {
        _curDefense = _defense;
        OnPawnClicked?.Invoke(false, null);
        GameManager.Instance.TurnEnd();
    }
    public override void UseSkill()
    {
        _skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        _skill.UseSkill();
    }
    public override void TakeDamage(int damage)
    {
        damage -= _curDefense;
        _curHealth -= damage;
        var damParticle = ObjectManager.Instance.SpawnParticle(PawnManager.Instance._damageTextParticle, StringKeys.DAMAGE, true);
        var damageText = damParticle.GetComponent<DamageText>();
        var spawnPosition = this.transform.position;
        damageText.SetText(damage, spawnPosition, false);
        _curDefense = 0;
        UpdateHpBar();
        
        if (_curHealth <= 0)
            Die();
    }
    protected override void Die()
    {
        Debug.Log($"{this.gameObject.name} is dead");
        OnDie?.Invoke(this);
    }
}
