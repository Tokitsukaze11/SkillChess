using System;
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
        //_skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        /*_skill = new AttackDecorator(this,20,5,AttackType.ConsiderOtherPawnTarget);
        (_skill as AttackDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        /*_skill = new HowitzerDecorator(this, 20, 5, 5);
        (_skill as HowitzerDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };*/
        _skill = new HealDecorator(this, 10, 5,HealType.Single);
        (_skill as HealDecorator)!.OnSkillEnd += () =>
        {
            OnPawnClicked?.Invoke(false, null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        /*_skill = new DefendDecorator(this, 5);
        (_skill as DefendDecorator)!.OnSkillEnd += () =>
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
        //this.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Toon/Toon Complete"));
        /*var curRenderer = this.GetComponent<Renderer>();
        _materials.Clear();
        _materials.AddRange(curRenderer.sharedMaterials);
        _materials.Add(new Material(Shader.Find("Hidden/Outline")));
        curRenderer.materials = _materials.ToArray();*/
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
        targetSquares.Where(x => !x.IsAnyPawn() && !x.IsObstacle).ToList().ForEach(x =>
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
        var path = MoveNavigation.FindNavigation(_curMapSquare, _moveTargetSquare);
        
        Queue<Vector2> pathKeys = new Queue<Vector2>();
        foreach (var mapSquare in path)
        {
            var key = SquareCalculator.CurrentKey(mapSquare);
            pathKeys.Enqueue(key);
        }
        
        Action callBack = () =>
        {
            _curMapSquare.CurPawn = null;
            _curMapSquare = _moveTargetSquare;
            _moveTargetSquare.CurPawn = this;
            int curKeyIndexInt = SquareCalculator.CurrentIndex(_curMapSquare);
            _skill.UpdateCurIndex(curKeyIndexInt);
            OnPawnClicked?.Invoke(false,null);
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        
        if (pathKeys.Count > 0)
        {
            StartCoroutine(Co_Move(pathKeys, callBack));
        }
        return;
    }
    public override IEnumerator Co_Move(Queue<Vector2> path, Action callback)
    {
        List<Vector2> pathList = path.ToList();
        // 이동 경로에서 각각의 꼭지점을 찾기.
        Queue<Vector2> vertex = new Queue<Vector2>();
        var curPath = pathList[0];
        vertex.Enqueue(curPath);
        for (int i = 0; i < pathList.Count; i++)
        {
            var key = pathList[i];
            float x = key.x;
            float y = key.y;
            if (Mathf.Approximately(x, curPath.x) || Mathf.Approximately(y, curPath.y))
                continue;
            vertex.Enqueue(pathList[i - 1]);
            curPath = pathList[i];
        }
        if(!vertex.Contains(pathList[^1]))
            vertex.Enqueue(pathList[^1]);
        vertex.Dequeue(); // 시작점 제거
        float time = 0.5f + Math.Clamp((vertex.Count - 1) * 0.1f, 0, 0.5f);
        yield return new WaitForSeconds(0.3f);
        while (vertex.Count > 0)
        {
            var key = vertex.Dequeue();
            var target = new Vector3(key.x, 1, key.y);
            this.transform.DOMove(target, time);
            // TODO : 이동 애니메이션 추가
            yield return new WaitForSeconds(time);
        }
        callback!();
        yield break;
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
        targetSquares.Where(x => x.IsAnyPawn() && !x.IsObstacle ).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList().ForEach(x =>
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
        base.TakeDamage(damage);
        if (_curHealth <= 0)
            Die();
    }
    protected override void Die()
    {
        Debug.Log($"{this.gameObject.name} is dead");
        OnDie?.Invoke(this);
    }
}
