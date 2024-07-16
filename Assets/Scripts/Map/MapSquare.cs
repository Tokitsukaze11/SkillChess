using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MapSquare : MonoBehaviour // TODO : Check it will be abstract
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material[] _colorMaterials;
    private bool _isAnyPawn; // 다른 Pawn이 차지하고 있는지 여부
    private bool _isChoosen;
    private bool _isObstacle; // 장애물이 있는지 여부, Pawn 제외
    public event Action<MapSquare> OnClickSquare;
    private Pawn _curPawn;
    private Coroutine _mouseOverCoroutine;
    private const int RED_COLOUR = 0;
    private const int YELLOW_COLOUR = 1;
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
        return _isAnyPawn;
    }
    public bool IsObstacle
    {
        get => _isObstacle;
        set => _isObstacle = value;
    }
    public void SetColor(Color color, bool isDirect = false)
    {
        if (color == GlobalValues.SELECABLE_COLOUR)
        {
            if (isDirect)
            {
                _meshRenderer.material = _colorMaterials[YELLOW_COLOUR];
                return;
            }
            var newMat = new Material(_colorMaterials[RED_COLOUR]);
            _meshRenderer.material = newMat;
            _meshRenderer.material.DOColor(color, 0.3f).onComplete = () => _meshRenderer.material = _colorMaterials[YELLOW_COLOUR];
        }
        else
            _meshRenderer.material = _colorMaterials[RED_COLOUR];
        _isChoosen = color == GlobalValues.SELECABLE_COLOUR;
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
        SetColor(GlobalValues.SELECABLE_COLOUR, true);
    }
    private IEnumerator Co_ColourFade()
    {
        yield return new WaitForSeconds(0.5f);
        float time = 0;
        var newMat = new Material(_colorMaterials[YELLOW_COLOUR]);
        _meshRenderer.material = newMat;
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
        if(_mouseOverCoroutine != null)
            StopCoroutine(_mouseOverCoroutine);
        SetColor(GlobalValues.UNSELECT_COLOUR);
        OnClickSquare = null;
    }
}
