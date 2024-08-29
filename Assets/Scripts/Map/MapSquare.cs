using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MapSquare : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material[] _colorMaterials;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _isAnyPawn; // 다른 Pawn이 차지하고 있는지 여부
    private bool _isObstacle; // 장애물이 있는지 여부, Pawn 제외
    public event Action<MapSquare> OnClickSquare;
    private Pawn _curPawn;
    private Coroutine _mouseOverCoroutine;
    private Color _originColor;
    public Pawn CurPawn
    {
        set
        {
            _curPawn = value;
        }
        get => _curPawn;
    }
    public Obstacle Obstacle { get; set; }
    public bool IsAnyPawn()
    {
        return _curPawn != null;
    }
    public bool IsObstacle
    {
        get => _isObstacle;
        set => _isObstacle = value;
    }
    public void SetColor(Color color, bool isDirect = false)
    {
        if (color == GlobalValues.UNSELECT_COLOUR)
        {
            _spriteRenderer.color = GlobalValues.UNSELECT_COLOUR;
            return;
        }
        if (isDirect)
            _spriteRenderer.color = color;
        else
            _spriteRenderer.DOColor(color, 0.3f).onComplete = () => _spriteRenderer.color = color;
        _originColor = color;
    }
    #region RayCast Event
    public void OnMouseClick(Action correctionAction)
    {
        if(OnClickSquare == null)
        {
            if(ReferenceEquals(_curPawn,null))
                correctionAction?.Invoke();
            else
                _curPawn.OnMouseClick();
            return;
        }
        if(_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
            _mouseOverCoroutine = null;
        }
        OnClickSquare?.Invoke(this);
        
    }
    public void OnMouseEnterCast(Action callBack)
    {
        if (OnClickSquare == null)
            return;
        _mouseOverCoroutine = StartCoroutine(Co_ColourFade());
        // 장애물들 체크해서 투명하게 하기
        callBack?.Invoke();
    }
    public void OnMouseExitCast(Action callBack)
    {
        if(OnClickSquare == null)
            return;
        if (_mouseOverCoroutine == null)
            return;
        StopCoroutine(_mouseOverCoroutine);
        _mouseOverCoroutine = null;
        SetColor(_originColor, true);
        // 장애물들 체크해서 원래대로 돌리기
        callBack?.Invoke();
    }
    #endregion
    #region Unity Event
    public void OnMouseDown() // 레이케스트 방식으로 변경해야 함(장애물이 앞에 있으면 클릭이 안됨 -> 불쾌한 경험 해결)
    {
        /*if (!_isChoosen)
            return;
        if(_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
            _mouseOverCoroutine = null;
        }
        OnClickSquare?.Invoke(this);*/
    }
    public void OnMouseEnter()
    {
        /*if(_isChoosen)
            _mouseOverCoroutine = StartCoroutine(Co_ColourFade());*/
    }
    public void OnMouseExit()
    {
        /*if (!_isChoosen)
            return;
        if (_mouseOverCoroutine == null)
            return;
        StopCoroutine(_mouseOverCoroutine);
        _mouseOverCoroutine = null;
        SetColor(_originColor, true);*/
    }
  #endregion
    private IEnumerator Co_ColourFade()
    {
        yield return new WaitForSeconds(0.5f);
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            //_meshRenderer.material.color = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(time, 1));
            _spriteRenderer.color = Color.Lerp(_originColor, GlobalValues.UNSELECT_COLOUR, Mathf.PingPong(time, 1));
            yield return null;
        }
    }
    public void ResetColor()
    {
        if(_mouseOverCoroutine != null)
            StopCoroutine(_mouseOverCoroutine);
        SetColor(GlobalValues.UNSELECT_COLOUR);
        OnClickSquare = null;
    }
}
