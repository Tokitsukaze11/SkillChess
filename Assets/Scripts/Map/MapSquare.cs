using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MapSquare : MonoBehaviour // TODO : Check it will be abstract
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private bool _isAnyPawn; // 다른 Pawn이 차지하고 있는지 여부
    private bool _isChoosen;
    private bool _isObstacle; // 장애물이 있는지 여부, Pawn 제외
    public event Action<MapSquare> OnClickSquare;
    private Pawn _curPawn;
    private Coroutine _mouseOverCoroutine;
    public Pawn CurPawn
    {
        set
        {
            _curPawn = value;
            _isAnyPawn = _curPawn != null;
        }
        get => _curPawn;
    }
    public bool IsAnyPawn()
    {
        return !_isAnyPawn;
    }
    public bool IsObstacle
    {
        get => _isObstacle;
        set => _isObstacle = value;
    }
    public void SetColor(Color color)
    {
        _meshRenderer.material.color = color;
        _isChoosen = color == Color.yellow;
    }
    public void OnMouseDown()
    {
        if (!_isChoosen)
            return;
        if(_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
            _mouseOverCoroutine = null;
        }
        OnClickSquare?.Invoke(this);
    }
    public void OnMouseEnter()
    {
        if(_isChoosen)
            _mouseOverCoroutine = StartCoroutine(Co_ColourFade());
    }
    public void OnMouseExit()
    {
        if (!_isChoosen)
            return;
        if (_mouseOverCoroutine == null)
            return;
        StopCoroutine(_mouseOverCoroutine);
        _mouseOverCoroutine = null;
        SetColor(Color.yellow);
    }
    private IEnumerator Co_ColourFade()
    {
        yield return new WaitForSeconds(0.5f);
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            _meshRenderer.material.color = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(time, 1));
            yield return null;
        }
        yield break;
    }
    public bool IsCanClick()
    {
        return _isChoosen;
    }
    public void ResetColor()
    {
        SetColor(Color.red);
        OnClickSquare = null;
    }
}
