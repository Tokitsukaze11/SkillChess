using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MapSquare : MonoBehaviour // TODO : Check it will be abstract
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private bool _isOccupied; // 다른 Pawn이 차지하고 있는지 여부
    private bool _isChoosen;
    private bool _isObstacle; // 장애물이 있는지 여부, Pawn 제외
    public event Action<MapSquare> OnClickSquare;
    private Pawn _curPawn;
    public Pawn CurPawn
    {
        set
        {
            _curPawn = value;
            _isOccupied = _curPawn != null;
        }
        get => _curPawn;
    }
    public bool IsCanMove()
    {
        return !_isOccupied;
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
        OnClickSquare?.Invoke(this);
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
