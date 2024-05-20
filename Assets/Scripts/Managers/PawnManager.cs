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
            /*CheckDirection(curKeyIndex + (i*8), 63, keys, targetSquares); //Right
            CheckDirection(curKeyIndex - (i*8), 0, keys, targetSquares); //Left
            CheckDirection(curKeyIndex + i, 63, keys, targetSquares); //Up
            CheckDirection(curKeyIndex - i, 0, keys, targetSquares); //Down*/
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
            if (newSquare != null && newSquare.IsCanMove())
                targetSquares.Add(newSquare);
        }
    }
    #region Original CheckTargetSquares
    /*public void CheckTargetSquares(int movementRange, int curKeyIndex, ref List<MapSquare> targetSquares)
    {
        var keys = _mapSquareDic.Keys.ToList();
        for (int i = 1; i <= movementRange; i++)
        {
            // 오른쪽으로 간다 = x값이 증가한다 = 원래는 i,j로 되어 있던게 index로 변함, 0~63.
            // i*8 + j 이렇게 저장되어 있음. 여기서 오른쪽으로 간다는 거는 i*8이 변한다는 것. 그래서 i가 증가함. 결국에는 이전과는 index의 차이가 8이 나는 것.
            //여기서 moveRange만큼 이동 가능하다는 말은 1 ~ moveRange까지 이동 가능하다는 것. for문에서 i가 1부터 증가하니까
            //즉, i가 1 증가할 때 마다 8씩 증가. 그렇다면 해당 숫자 만큼 증가 시킨 값이 64보다 작으면 이동 가능한 것.
            if(curKeyIndex + (i*8) < 64) //Right
            {
                var newKey = keys[curKeyIndex + (i*8)];
                var newSquare = PawnManager.Instance.GetCurrentMapSquare(newKey);
                if (newSquare != null)
                    targetSquares.Add(newSquare);
            }
            if (curKeyIndex - (i*8) >= 0) //Left
            {
                var newKey = keys[curKeyIndex - (i*8)];
                var newSquare = PawnManager.Instance.GetCurrentMapSquare(newKey);
                if (newSquare != null)
                    targetSquares.Add(newSquare);
            }
            // 위로 간다 = y값이 증가한다 = 이번에는 j값이 증가한다. j만 증가하면 되니까 i는 그대로.
            //for문을 기준으로 한다면 i만큼 증가시키면 된다.
            if (curKeyIndex + i < 64) //Up
            {
                var newKey = keys[curKeyIndex + i];
                var newSquare = PawnManager.Instance.GetCurrentMapSquare(newKey);
                if (newSquare != null)
                    targetSquares.Add(newSquare);
            }
            if (curKeyIndex - i >= 0) //Down
            {
                var newKey = keys[curKeyIndex - i];
                var newSquare = PawnManager.Instance.GetCurrentMapSquare(newKey);
                if (newSquare != null)
                    targetSquares.Add(newSquare);
            }
        }
    }*/
  #endregion
}
