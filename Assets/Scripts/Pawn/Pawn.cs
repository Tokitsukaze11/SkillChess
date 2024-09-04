using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public enum PawnType
{
    Pawn,
    King,
}
public enum MoveType
{
    Straight,
    Range,
}

public abstract class Pawn : MonoBehaviour
{
    //protected List<Material> _materials = new List<Material>();
    public bool _isPlayerPawn;
    [HideInInspector] public bool _isCanClick = false;
    [Header("Pawn Status")]
    [SerializeField] protected int _health;
    [SerializeField] protected int _curHealth;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _defense;
    [SerializeField] protected int _shield = 0;
    protected int _curDefense = 0;
    [SerializeField] protected int _movementRange;
    [SerializeField] protected int _attackRange;
    [SerializeField] protected PawnType _pawnType;
    [Tooltip("0:Move,1:Attack,2:Defend,3:Skill")]
    public DescriptObject[] _descriptObjects;
    public Sprite _skillImage;
    [SerializeField] protected OutlineFx.OutlineFx[] _outlineFx;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected ObjectTriggerAnimation _objectTriggerAnimation;
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    [Header("UI")]
    [SerializeField] protected Transform _hpBarTransform;
    [SerializeField] protected SpriteRenderer _hpBar;
    [SerializeField] protected SpriteRenderer _hpBarRed;
    [SerializeField] protected SpriteRenderer _hpBarShield;
    public SortingGroup _sortingGroup;
    // Variables
    protected bool _isHowitzerAttack = false;
    protected MoveType _moveType;
    protected bool _isConsiderObstacle = true;
    protected string _attackParticleID;
    protected string _skillParticleID;
    private Camera _mainCamera;
    private MapSquare _curMapSquare;
    protected SkillDecorator _skill;
    private MapSquare _moveTargetSquare;
    protected AudioClip[] _hitSound;
    // Properties
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
    public ObjectTriggerAnimation ObjectTriggerAnimation => _objectTriggerAnimation;
    // Events
    public event Action<bool,Pawn> OnPawnClicked;
    public event Action<Pawn> OnDie;
    public event Action OnPlayDieSound;
    public event Action<int> OnCannotAction;
    public event Action OnHowitzerAttack;
    public event Action OnCannotClick;
    // static variables
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    private static readonly int Skill1 = Animator.StringToHash("Skill");
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int Die1 = Animator.StringToHash("Die");
    protected virtual void Awake()
    {
        _curHealth = _health;
        _hpBar.gameObject.transform.localScale = Vector3.one;
        _mainCamera = GameManager.Instance.mainCamera;
        var outLines = GetComponentsInChildren<OutlineFx.OutlineFx>();
        _outlineFx = outLines;
    }
    protected void Start()
    {
        // TODO : 이벤트가 null이 아닌지 확실히 확인 필요
        _skill.OnSkillUsed += OnPawnClicked;
    }
    public void SetNavMeshAgentDisable()
    {
        _navMeshAgent.enabled = false;
    }
    public IEnumerator Co_MoveToDest(Vector3 destination)
    {
        yield return new WaitForSeconds(0.2f);
        _navMeshAgent.SetDestination(destination);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool(Run,true);
        
        while (_navMeshAgent.enabled)
        {
            if (_navMeshAgent.remainingDistance <= 0.1f)
            {
                _animator.SetBool(Run, false);
                _navMeshAgent.ResetPath();
                _navMeshAgent.enabled = false;
                this.gameObject.transform.position = destination;
                this.gameObject.transform.rotation = _isPlayerPawn ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
                yield break;
            }
            yield return null;
        }
        yield break;
    }
    private void Update()
    {
        var camRotation = _mainCamera.transform.rotation.eulerAngles;
        _hpBarTransform.rotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);
    }
    public void OnMouseClick()
    {
        // 보정은 미리 하고 들어옴
        if (!_isPlayerPawn || !_isCanClick)
        {
            OnCannotClick?.Invoke();
            return;
        }
        OnPawnClicked?.Invoke(true, this);
        PawnManager.Instance.ResetSquaresColor();
        _outlineFx.ToList().ForEach(x => x.enabled = true);
    }
    public void OnOutlineDisable()
    {
        _outlineFx.ToList().ForEach(x => x.enabled = false);
    }
    public virtual void ShowMoveRange()
    {
        PawnManager.Instance.ResetSquaresColor();
        var targetSquares = new List<MapSquare>();
        int curKeyIndex = SquareCalculator.CurrentIndex(_curMapSquare);
        if(_moveType == MoveType.Straight)
            SquareCalculator.CheckTargetSquares(_movementRange, curKeyIndex, targetSquares, _isConsiderObstacle); // 직선 이동
        else
            SquareCalculator.CheckTargetSquaresAsRange(_movementRange, _curMapSquare, targetSquares); // 거리 우선 이동
        targetSquares = targetSquares.Where(x => !x.IsAnyPawn() && !x.IsObstacle).ToList();
        if(targetSquares.Count == 0)
        {
            OnCannotAction?.Invoke(0);
            return;
        }
        targetSquares.ForEach(x =>
        {
            x.SetColor(GlobalValues.MOVEABLE_COLOUR);
            x.OnClickSquare += (mapSquare) =>
            {
                _moveTargetSquare = mapSquare;
                Move();
            };
        });
    }
    protected virtual void Move()
    {
        PawnManager.Instance.ResetSquaresColor(); // MapSquare의 색상을 초기화와 동시에 대리자 초기화
        
        _outlineFx.ToList().ForEach(x => x.enabled = false);
        
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
            Observable.FromCoroutine(() => Co_Move(path, callBack)).Subscribe().AddTo(this);
        }
        return;
    }
    public void MoveOrder(Queue<MapSquare> path, Action callback)
    {
        Observable.FromCoroutine(() => Co_Move(path, callback)).Subscribe().AddTo(this);
    }
    protected virtual IEnumerator Co_Move(Queue<MapSquare> path, Action callback)
    {
        List<int> pathIndexList = path.Select(SquareCalculator.CurrentIndex).ToList();
        Queue<int> vertexIndex = new Queue<int>();
        var curIndex = SquareCalculator.CurrentIndex(_curMapSquare);
        vertexIndex.Enqueue(curIndex);
        for (int i = 0; i < pathIndexList.Count; i++)
        {
            var index = pathIndexList[i];
            int nextRow = index % GlobalValues.ROW;
            int nextCol = index / GlobalValues.ROW;
            int curRow = curIndex % GlobalValues.ROW;
            int curCol = curIndex / GlobalValues.ROW;
            if (nextRow == curRow || nextCol == curCol)
                continue;
            vertexIndex.Enqueue(pathIndexList[i - 1]);
            curIndex = pathIndexList[i -1];
        }
        if (!vertexIndex.Contains(pathIndexList[^1]))
            vertexIndex.Enqueue(pathIndexList[^1]);
        vertexIndex.Dequeue(); // 시작점 제거
        float time = 0.5f + Math.Clamp((vertexIndex.Count - 1) * 0.1f, 0, 0.5f);
        yield return new WaitForSeconds(0.3f);
        while (vertexIndex.Count > 0)
        {
            var index = vertexIndex.Dequeue();
            var target = SquareCalculator.CurrentKey(index);
            this.transform.rotation = Quaternion.LookRotation(new Vector3(target.x, 0, target.y) - this.transform.position);
            _animator.SetBool(Run, true);
            this.transform.DOMove(new Vector3(target.x, 0, target.y), time).onComplete = () =>
            {
                _animator.SetBool(Run, false);
                this.transform.rotation = GameManager.Instance.IsPlayer1Turn.Invoke() ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            };
            yield return new WaitForSeconds(time);
        }
        callback!();
        yield break;
    }
    public virtual void ShowAttackRange()
    {
        PawnManager.Instance.ResetSquaresColor();

        var targetSquares = new List<MapSquare>();
        var curKeyIndex = SquareCalculator.CurrentIndex(_curMapSquare);

        if (_isHowitzerAttack)
            SquareCalculator.CheckTargetSquares(_attackRange, curKeyIndex, targetSquares);
        else
            SquareCalculator.CheckTargetSquares(_attackRange, curKeyIndex, targetSquares, true);
        var rangeSquares = targetSquares.ToList();
        targetSquares = targetSquares.Where(x => x.IsAnyPawn() && !x.IsObstacle).ToList().Where(x => !x.CurPawn._isPlayerPawn).ToList();
        if(targetSquares.Count == 0)
        {
            OnCannotAction?.Invoke(1);
            return;
        }
        rangeSquares = rangeSquares.Except(targetSquares).ToList();
        rangeSquares.Where(x => !x.IsObstacle).ToList().ForEach(x=>x.SetColor(GlobalValues.ATTACKABLE_COLOUR));
        targetSquares.ForEach(x =>
        {
            x.SetColor(GlobalValues.ATTACKABLE_COLOUR);
            x.OnClickSquare += (mapSquare) =>
            {
                Attack(x.CurPawn, x);
            };
        });
    }
    protected virtual void Attack(Pawn targetPawn, MapSquare targetSquare)
    {
        PawnManager.Instance.ResetSquaresColor();
        
        _outlineFx.ToList().ForEach(x => x.enabled = false);

        _objectTriggerAnimation.OnAnimationTrigger += () =>
        {
            SoundManager.Instance.PlaySfx(_hitSound[Random.Range(0, _hitSound.Length)]);
            targetPawn.TakeDamage(_damage,_attackParticleID);
            _curDefense = 0;
            _objectTriggerAnimation.ResetTrigger();
        };
        OnPawnClicked?.Invoke(false, null);
        Observable.FromCoroutine(() => Co_Attack(targetSquare)).Subscribe().AddTo(this);
    }
    private IEnumerator Co_Attack(MapSquare targetSquare)
    {
        yield return new WaitForSeconds(0.5f);
        if (_isHowitzerAttack)
        {
            _objectTriggerAnimation.OnAnimationEndTrigger += OnHowitzerAttack;
            this.transform.rotation = Quaternion.LookRotation(new Vector3(targetSquare.transform.position.x, 0, targetSquare.transform.position.z) - this.transform.position);
            _animator.SetTrigger(Attack1);
            yield return new WaitForSeconds(1.2f);
            //this.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            this.transform.rotation = GameManager.Instance.IsPlayer1Turn.Invoke() ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            GameManager.Instance.TurnEnd();
            yield break;
        }
        var curSq = _curMapSquare;
        var attackPath = MoveNavigation.FindNavigation(curSq, targetSquare);
        var realPath = new Queue<MapSquare>(attackPath.SkipLast(1));
        var reversPathQueue = new Queue<MapSquare>();
        for(int i = realPath.Count - 1; i >= 0; i--)
        {
            reversPathQueue.Enqueue(realPath.ElementAt(i));
        }
        _objectTriggerAnimation.OnAnimationEndTrigger += () =>
        {
            _curMapSquare = realPath.Last();
            Observable.FromCoroutine(() => Co_Move(reversPathQueue, () =>
            {
                _curMapSquare = curSq;
                _objectTriggerAnimation.ResetEndTrigger();
                this.transform.rotation = GameManager.Instance.IsPlayer1Turn.Invoke() ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
                GameManager.Instance.TurnEnd();
            })).Subscribe().AddTo(this);
        };
        Observable.FromCoroutine(() => Co_Move(realPath, () =>
        {
            this.transform.rotation = Quaternion.LookRotation(new Vector3(targetSquare.transform.position.x, 0, targetSquare.transform.position.z) - this.transform.position);
            _animator.SetTrigger(Attack1);
        })).Subscribe().AddTo(this);
        yield break;
    }
    public virtual void Defend()
    {
        PawnManager.Instance.ResetSquaresColor();
        _outlineFx.ToList().ForEach(x => x.enabled = false);
        _curDefense = _defense;
        OnPawnClicked?.Invoke(false, null);
        GameManager.Instance.TurnEnd();
    }
    public virtual void UseSkill()
    {
        _skill.UpdateCurIndex(SquareCalculator.CurrentIndex(_curMapSquare));
        _skill.UseSkill();
    }
    public virtual void TakeDamage(int damage, string particleID = null)
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
        _animator.SetTrigger(Damage);
        var damParticle = ObjectManager.Instance.SpawnParticle(PawnManager.Instance._damageTextParticle, StringKeys.DAMAGE, true);
        var damageText = damParticle.GetComponent<DamageText>();
        var spawnPosition = this.transform.position;
        damageText.SetText(damage, spawnPosition, false);
        var hitParticle = ObjectManager.Instance.SpawnParticleViaID(particleID);
        Vector3 spawnPoint = spawnPosition + new Vector3(0, 0.2f, 0);
        hitParticle.transform.position = spawnPoint;
        hitParticle.SetActive(true);
        UpdateHpBar();
    }
    protected virtual void Die()
    {
        _isCanClick = false;
        _objectTriggerAnimation.OnAnimationTrigger += () =>
        {
            OnDie?.Invoke(this);
        };
        _animator.SetBool(Die1,true);
        OnPlayDieSound?.Invoke();
    }
    private void UpdateHpBar()
    {
        var curHpBarX = _hpBar.transform.localScale.x;
        var targetHpBarX = (float)_curHealth / _health;
        var shieldBarX = Mathf.Clamp01((float)_shield / _health);
        _hpBarShield.transform.localScale = new Vector3(shieldBarX, 1, 1);
        if (_curHealth <= 0)
        {
            _hpBar.transform.localScale = new Vector3(0, 1, 1);
            _hpBarRed.transform.DOScaleX(0, 0.5f).SetDelay(0.2f);
            Die();
            return;
        }
        if(curHpBarX > targetHpBarX) // 데미지를 받은 경우
        {
            _hpBar.transform.localScale = new Vector3(targetHpBarX, 1, 1);
            _hpBarRed.transform.DOScaleX(targetHpBarX, 0.5f).SetDelay(1f);
        }
        else // 힐을 받은 경우
        {
            _hpBar.transform.DOScaleX(targetHpBarX, 0.5f).SetDelay(0.5f).onComplete = () =>
            {
                _hpBarRed.transform.localScale = new Vector3((float)_curHealth / _health, 1, 1);
            };
        }
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
    public void SkillAnimation()
    {
        _animator.SetTrigger(Skill1);
    }
    public void ResetOutline()
    {
        _outlineFx.ToList().ForEach(x => x.enabled = false);
    }
    public void CannotUseSkill()
    {
        OnCannotAction?.Invoke(3);
    }
}
