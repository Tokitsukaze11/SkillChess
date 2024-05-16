using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSquare : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private Pawn _curPawn;
    public Pawn CurPawn
    {
        set
        {
            _curPawn = value;
            _isOccupied = _curPawn != null;
        }
    }
    private bool _isOccupied;
    public bool IsCanMove()
    {
        return !_isOccupied;
    }
    public void SetColor(Color color)
    {
        _meshRenderer.material.color = color;
    }
    public void ResetColor()
    {
        _meshRenderer.material.color = Color.red;
    }
}
