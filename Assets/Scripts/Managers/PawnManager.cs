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
    public void TurnChange(bool isPlayerTurn)
    {
        _playerPawnController.TurnChange(isPlayerTurn);
        //_enemyPawnController.TurnChange(!isPlayerTurn);
    }
    public void ResetSquaresColor()
    {
        _mapSquareDic.Values.ToList().ForEach(x => x.ResetColor());
    }
    #region Check Target Squares
    /// <summary>
    /// Check squares that can something be done
    /// </summary>
    /// <param name="targetRange">Range of do something</param>
    /// <param name="curKeyIndex">Current key index</param>
    /// <param name="targetSquares">List of target squares</param>
    /// <param name="isConsideringObstacles">Is considering obstacles to do. If true, try to stop when obstacle is found</param>
    public void CheckTargetSquares(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        var keys = _mapSquareDic.Keys.ToList();
        GetVerticalCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles);
        GetHorizontalCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles);
    }
    private void GetVerticalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        int n = 8;
        int m = 8;
        
        int nowCol = curKeyIndex / n;
        
        int maxBoundary = n * (nowCol + 1) - 1;
        int minBoundary = n * nowCol;
        
        /*// Check Up Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex + i;
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
            if(isMove && IsOverlapped(i, targetRange, targetSquares))
                break;
        }
        // Check Down Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex - i;
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
            if(isMove && IsOverlapped(i, targetRange, targetSquares))
                break;
        }*/
        
        // Make Less Cognitive Complexity
        int[] directions = { 1, -1 }; // 위, 아래 방향

        foreach (int direction in directions)
        {
            int currentIndex = 0;
            while (currentIndex < targetRange)
            {
                int newKeyIndex = curKeyIndex + (currentIndex + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);

                if (isConsideringObstacles && IsOverlapped(currentIndex + 1, targetRange, targetSquares))
                    break;

                currentIndex++;
            }
        }
        
        // n*m (n 행 m 열)
        // 나의 열에서의 최대 값 : n*(현재 열+1)-1
        // 나의 열에서의 최소 값 : n*현재 열
    }
    private void GetHorizontalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        int n = 8;
        int m = 8;

        int nowRow = curKeyIndex % n;
        
        int minBoundary = nowRow;
        int maxBoundary = curKeyIndex + (n * targetRange);
        
        /*// Check Right Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex + (i * n);
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
            if(isMove && IsOverlapped(i, targetRange, targetSquares))
                break;
        }
        // Check Left Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex - (i * n);
            CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
            if(isMove && IsOverlapped(i, targetRange, targetSquares))
                break;
        }*/
        
        // Make Less Cognitive Complexity
        int[] directions = { n, -n }; // 오른쪽, 왼쪽 방향

        foreach (int direction in directions)
        {
            int currentIndex = 0;
            while (currentIndex < targetRange)
            {
                int newKeyIndex = curKeyIndex + (currentIndex + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);

                if (isConsideringObstacles && IsOverlapped(currentIndex + 1, targetRange, targetSquares))
                    break;

                currentIndex++;
            }
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
    private bool IsOverlapped(int curIndex, int range, List<MapSquare> targetSquares)
    {
        if (curIndex < range)
        {
            if (targetSquares.Count > 0)
                return !targetSquares[^1].IsCanMove();
        }
        return false;
    }
  #endregion
}
