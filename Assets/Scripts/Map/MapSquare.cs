using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSquare : MonoBehaviour // TODO : Check it will be abstract
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private bool _isOccupied;
    private bool _isChoosen;
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
    public void SetColor(Color color)
    {
        _meshRenderer.material.color = color;
        _isChoosen = color == Color.yellow;
    }
    public void OnMouseUp()
    {
        if (!_isChoosen)
            return;
        OnClickSquare?.Invoke(this);
    }
    public void ResetColor()
    {
        _meshRenderer.material.color = Color.red;
        OnClickSquare = null;
    }
}
