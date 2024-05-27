using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PawnManager : Singleton<PawnManager>
{
    public GameObject _damageTextParticle;
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
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_damageTextParticle, StringKeys.DAMAGE);
    }
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
    #region Check Target Squares
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
        
        int nowCol = curKeyIndex / n;
        
        int maxBoundary = n * (nowCol + 1) - 1;
        int minBoundary = n * nowCol;
        
        // Check Up Direction
        for(int i = 1; i <= moveRange; i++)
        {
            int newKeyIndex = curKeyIndex + i;
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
        }
        // Check Down Direction
        for(int i = 1; i <= moveRange; i++)
        {
            int newKeyIndex = curKeyIndex - i;
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
        }

        // n*m (n 행 m 열)
        // 나의 열에서의 최대 값 : n*(현재 열+1)-1
        // 나의 열에서의 최소 값 : n*현재 열
    }
    private void GetHorizontalMoves(int moveRange, int curKeyIndex, List<MapSquare> targetSquares)
    {
        int n = 8;
        int m = 8;

        int nowRow = curKeyIndex % n;
        
        int minBoundary = nowRow;
        int maxBoundary = curKeyIndex + (n * moveRange);
        
        // Check Right Direction
        for(int i = 1; i <= moveRange; i++)
        {
            int newKeyIndex = curKeyIndex + (i * n);
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
        }
        // Check Left Direction
        for(int i = 1; i <= moveRange; i++)
        {
            int newKeyIndex = curKeyIndex - (i * n);
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
        }

        // n*m (n 행 m 열)
        // 나의 행에서 이동할 수 있는 거리만큼의 최대 값 : 현재 인덱스 + (n*이동할 수 있는 거리)
        // 나의 행에서 이동할 수 있는 거리만큼의 최소 값 : 현재 행의 인덱스
    }
    private void CheckDirection(int newKeyIndex, int minBoundary, int maxBoundary, List<Vector2> keys, List<MapSquare> targetSquares)
    {
        if ((newKeyIndex >= minBoundary && newKeyIndex < keys.Count) && (newKeyIndex <= maxBoundary && newKeyIndex >= 0))
        {
            var newKey = keys[newKeyIndex];
            var newSquare = GetCurrentMapSquare(newKey);
            if (newSquare != null)
                targetSquares.Add(newSquare);
        }
    }
  #endregion
}
