using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public enum PawnType
{
    Pawn,
    King,
}

public abstract class Pawn : MonoBehaviour
{
    //protected List<Material> _materials = new List<Material>();
    public bool _isPlayerPawn;
    public bool _isCanClick = false;
    [SerializeField] protected int _health;
    [SerializeField] protected int _curHealth;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _defense;
    [SerializeField] protected int _shield = 0;
    protected int _curDefense = 0;
    [SerializeField] protected int _movementRange;
    [SerializeField] protected int _attackRange;
    [SerializeField] protected PawnType _pawnType;
    public DescriptObject[] _descriptObjects;
    [SerializeField] protected Transform _hpBarTransform;
    [SerializeField] protected SpriteRenderer _hpBar;
    [SerializeField] protected SpriteRenderer _hpBarRed;
    [SerializeField] protected SpriteRenderer _hpBarShield;
    public SortingGroup _sortingGroup;
    private Camera _mainCamera;
    protected MapSquare _curMapSquare;
    protected SkillDecorator _skill;
    [SerializeField] protected OutlineFx.OutlineFx _outlineFx;
    public PawnType PawnType => _pawnType;
    public MapSquare CurMapSquare
    {
        set
        {
            _curMapSquare = value;
        }
        get
        {
            return _curMapSquare;
        }
    }
    public Action<bool,Pawn> OnPawnClicked;
    public Action<Pawn> OnDie;
    protected MapSquare _moveTargetSquare;
    protected void Awake()
    {
        _curHealth = _health;
        _hpBar.gameObject.transform.localScale = Vector3.one;
        _mainCamera = GameManager.Instance.mainCamera;
    }
    protected void Update()
    {
        var camRotation = _mainCamera.transform.rotation.eulerAngles;
        _hpBarTransform.rotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);
    }
    protected virtual void OnMouseDown()
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
        _outlineFx.enabled = true;
    }
    public void UnSelected()
    {
        _outlineFx.enabled = false;
    }
    public virtual void ShowMoveRange()
    {
        // Reset color
        PawnManager.Instance.ResetSquaresColor();
        // Check now values
        var targetSquares = new List<MapSquare>();
        int curKeyIndex = SquareCalculator.CurrentIndex(_curMapSquare);
        // Check target squares
        //SquareCalculator.CheckTargetSquares(_movementRange, curKeyIndex, targetSquares,true); // 직선 이동
        SquareCalculator.CheckTargetSquaresAsRange(_movementRange, _curMapSquare, targetSquares,true); // 거리 우선 이동
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
    protected virtual void Move()
    {
        if(!_isPlayerPawn)
            StartCoroutine(Co_EnemyMove());
        
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        _outlineFx.enabled = false;
        
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
            _curDefense = 0;
            GameManager.Instance.TurnEnd();
        };
        
        if (pathKeys.Count > 0)
        {
            OnPawnClicked?.Invoke(false,null);
            StartCoroutine(Co_Move(pathKeys, callBack));
        }
        return;
    }
    protected virtual IEnumerator Co_Move(Queue<Vector2> path, Action callback)
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
    protected abstract IEnumerator Co_EnemyMove();
    public virtual void ShowAttackRange()
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
    protected virtual void Attack(Pawn targetPawn)
    {
        if(!_isPlayerPawn)
            StartCoroutine(Co_EnemyMove());
        
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        _outlineFx.enabled = false;

        targetPawn.TakeDamage(_damage);
        OnPawnClicked?.Invoke(false, null);
        _curDefense = 0;
        GameManager.Instance.TurnEnd();
    }
    public virtual void Defend()
    {
        _outlineFx.enabled = false;
        _curDefense = _defense;
        OnPawnClicked?.Invoke(false, null);
        GameManager.Instance.TurnEnd();
    }
    public virtual void UseSkill()
    {
        _skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        _skill.UseSkill();
    }
    public virtual void TakeDamage(int damage)
    {
        damage -= _curDefense;
        damage -= _shield;
        if(damage < 0)
        {
            damage = 0;
            _shield = 0;
        }
        _curHealth -= damage;
        _curDefense = 0;
        var damParticle = ObjectManager.Instance.SpawnParticle(PawnManager.Instance._damageTextParticle, StringKeys.DAMAGE, true);
        var damageText = damParticle.GetComponent<DamageText>();
        var spawnPosition = this.transform.position;
        damageText.SetText(damage, spawnPosition, false);
        UpdateHpBar();
    }
    protected abstract void Die();
    private void UpdateHpBar()
    {
        var curHpBarX = _hpBar.transform.localScale.x;
        var targetHpBarX = (float)_curHealth / _health;
        var shieldBarX = Mathf.Clamp01((float)_shield / _health);
        _hpBarShield.transform.localScale = new Vector3(shieldBarX, 1, 1);
        if(curHpBarX > targetHpBarX) // 데미지를 받은 경우
        {
            _hpBar.transform.localScale = new Vector3(targetHpBarX, 1, 1);
            _hpBarRed.transform.DOScaleX(targetHpBarX, 0.5f).SetDelay(1f);
        }
        else // 힐을 받은 경우
        {
            //_hpBar.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
            //_hpBarRed.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
            _hpBar.transform.DOScaleX(targetHpBarX, 0.5f).SetDelay(0.5f).onComplete = () =>
            {
                _hpBarRed.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
            };
        }
        
        /*_hpBar.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
        _hpBarRed.transform.DOScaleX((float)_curHealth / _health, 0.5f).SetDelay(1f);*/
    }
    public void Heal(int healAmount)
    {
        _curHealth += healAmount;
        if (_curHealth > _health)
            _curHealth = _health;
        var healParticle = ObjectManager.Instance.SpawnParticle(PawnManager.Instance._healTextParticle, StringKeys.HEAL, true);
        var healText = healParticle.GetComponent<HealText>();
        var spawnPosition = this.transform.position;
        healText.SetText(healAmount, spawnPosition);
        UpdateHpBar();
    }
    public void GetShield(int shieldAmount)
    {
        _shield += shieldAmount;
        UpdateHpBar();
    }
}
