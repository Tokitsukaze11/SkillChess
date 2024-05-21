using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PawnManager : Singleton<PawnManager>
{
    private Vector2[] spawnPoints;
    public Vector2[] SpawnPoints
    {
        set
        {
            spawnPoints = value;
        }
    }
    private Dictionary<Vector2, MapSquare> _mapSquareDic;
    public Dictionary<Vector2, MapSquare> MapSquareDic
    {
        get => _mapSquareDic;
        set
        {
            _mapSquareDic = value;
        }
    }
    [SerializeField] private PlayerPawnController _playerPawnController;
    [SerializeField] private EnemyPawnController _enemyPawnController;
    public MapSquare GetCurrentMapSquare(Vector2 newKey)
    {
        var currentMapSquare = (from keyValuePair in _mapSquareDic let key = keyValuePair.Key 
            where Mathf.Approximately(key.x, newKey.x) && Mathf.Approximately(key.y, newKey.y) 
            select keyValuePair.Value).FirstOrDefault();
        return currentMapSquare;
    }
    public void SpawnPawn() // TODO : Will maybe get count of pawn
    {
        _playerPawnController.SpawnPlayerPawn(spawnPoints);
        _enemyPawnController.SpawnEnemyPawn(spawnPoints);
    }
    public void DespawnPawn()
    {
        
    }
    public void ResetSquaresColor()
    {
        _mapSquareDic.Values.ToList().ForEach(x => x.ResetColor());
    }
    public void CheckTargetSquares(int movementRange, int curKeyIndex, List<MapSquare> targetSquares)
    {
        var keys = _mapSquareDic.Keys.ToList();
        for (int i = 1; i <= movementRange; i++)
        {
            GetVerticalMoves(i, curKeyIndex, targetSquares);
            GetHorizontalMoves(i,curKeyIndex, targetSquares);
        }
    }
    private void GetVerticalMoves(int moveRange, int curKeyIndex, List<MapSquare> targetSquares)
    {
        int n = 8;
        int m = 8;
        
        int nowCol = curKeyIndex % n;
        
        // Check Up Direction
        if (nowCol < m - 1)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                int newKeyIndex = curKeyIndex + i;
                CheckDirection(newKeyIndex, n * m, _mapSquareDic.Keys.ToList(), targetSquares);
            }
        }
        // Check Down Direction
        if (nowCol > 0)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                int newKeyIndex = curKeyIndex - i;
                CheckDirection(newKeyIndex, n * m , _mapSquareDic.Keys.ToList(), targetSquares);
            }
        }

        // n*m (n 행 m 열)
        // 위 = +1 => each n*(i(0~n)+1)-1 (마지막 행)
        // 아래 = -1 => each n*i(0~n-1) (첫 행)
    }
    private void GetHorizontalMoves(int moveRange, int curKeyIndex, List<MapSquare> targetSquares)
    {
        int n = 8;
        int m = 8;

        int nowRow = curKeyIndex / n;
        
        // Check Right Direction
        if (nowRow < n - 1)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                int newKeyIndex = curKeyIndex + (i * n);
                CheckDirection(newKeyIndex, n * m, _mapSquareDic.Keys.ToList(), targetSquares);
            }
        }
        // Check Left Direction
        if (nowRow > 0)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                int newKeyIndex = curKeyIndex - (i * n);
                CheckDirection(newKeyIndex, n * m, _mapSquareDic.Keys.ToList(), targetSquares);
            }
        }

        // n*m (n 행 m 열)
        // 오른쪽 = +m => n*(m-1)~n*m-1 (마지막 열)
        // 왼쪽 = -m => n*0~n*(m-1) (첫 열)
    }
    private void CheckDirection(int newKeyIndex, int boundary, List<Vector2> keys, List<MapSquare> targetSquares)
    {
        if (newKeyIndex < boundary && newKeyIndex >= 0)
        {
            var newKey = keys[newKeyIndex];
            var newSquare = GetCurrentMapSquare(newKey);
            if (newSquare != null)
                targetSquares.Add(newSquare);
        }
    }
}
