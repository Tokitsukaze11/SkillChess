using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SquareCalculator
{
    private static Dictionary<Vector2, MapSquare> _mapSquareDic;
    public static Dictionary<Vector2, MapSquare> MapSquareDic
    {
        set
        {
            _mapSquareDic = value;
        }
    }
    #region Check Target Squares
    /// <summary>
    /// Check squares that can something be done
    /// </summary>
    /// <param name="targetRange">Range of do something</param>
    /// <param name="curKeyIndex">Current key index</param>
    /// <param name="targetSquares">List of target squares</param>
    /// <param name="isConsideringObstacles">Is considering obstacles to do. If true, try to stop when obstacle is found</param>
    public static void CheckTargetSquares(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        var keys = _mapSquareDic.Keys.ToList();
        GetVerticalCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles);
        GetHorizontalCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles);
    }
    /// <summary>
    /// Check squares that can something be done in diagonal direction
    /// </summary>
    /// <param name="targetRange">Range of do something</param>
    /// <param name="curKeyIndex">Current key index</param>
    /// <param name="targetSquares">List of target squares</param>
    /// <param name="isConsideringObstacles">Is considering obstacles to do. If true, try to stop when obstacle is found</param>
    public static void CheckDiagonalTargetSquares(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        var keys = _mapSquareDic.Keys.ToList();
        int row = GlobalValues.ROW; // n
        int col = GlobalValues.COL; // m
        
        int nowRow = curKeyIndex % row;
        int nowCol = curKeyIndex / row;
        
        /*// Check Right Up Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex + (i * n) + i;
            CheckDirection(newKeyIndex, nowRow, n * (nowCol + 1) - 1, keys, targetSquares);
            if(isConsideringObstacles && IsOverlapped(i, targetRange, targetSquares))
                break;
        }
        // Check Right Down Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex - (i * n) + i;
            CheckDirection(newKeyIndex, nowRow, n * (nowCol + 1) - 1, keys, targetSquares);
            if(isConsideringObstacles && IsOverlapped(i, targetRange, targetSquares))
                break;
        }
        // Check Left Up Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex + (i * n) - i;
            CheckDirection(newKeyIndex, n * nowCol, nowRow, keys, targetSquares);
            if(isConsideringObstacles && IsOverlapped(i, targetRange, targetSquares))
                break;
        }
        // Check Left Down Direction
        for(int i = 1; i <= targetRange; i++)
        {
            int newKeyIndex = curKeyIndex - (i * n) - i;
            CheckDirection(newKeyIndex, n * nowCol, nowRow, keys, targetSquares);
            if(isConsideringObstacles && IsOverlapped(i, targetRange, targetSquares))
                break;
        }*/
        
        int[] dx = { 1, 1, -1, -1 };
        int[] dy = { 1, -1, 1, -1 };

        for (int dir = 0; dir < 4; dir++)
        {
            int newRow = nowRow;
            int newCol = nowCol;

            for (int i = 1; i <= targetRange; i++)
            {
                newRow += dx[dir];
                newCol += dy[dir];

                if (newRow < 0 || newRow >= row || newCol < 0 || newCol >= col)
                    break;

                int newKeyIndex = newRow + newCol * row;
                CheckDirection(newKeyIndex, newRow, newCol, keys, targetSquares);

                if (isConsideringObstacles && IsOverlapped(i, targetRange, targetSquares))
                    break;
            }
        }
    }
    private static void GetVerticalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        int row = GlobalValues.ROW; // n
        int col = GlobalValues.COL; // m
        
        int nowCol = curKeyIndex / row;
        
        int maxBoundary = row * (nowCol + 1) - 1;
        int minBoundary = row * nowCol;
        
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
    private static void GetHorizontalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false)
    {
        int row = GlobalValues.ROW; // n
        int col = GlobalValues.COL; // m

        int nowRow = curKeyIndex % row;
        
        int minBoundary = nowRow;
        int maxBoundary = curKeyIndex + (row * targetRange);
        
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
        int[] directions = { row, -row }; // 오른쪽, 왼쪽 방향

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
    private static void CheckDirection(int newKeyIndex, int minBoundary, int maxBoundary, List<Vector2> keys, List<MapSquare> targetSquares)
    {
        if ((newKeyIndex >= minBoundary && newKeyIndex < keys.Count) && (newKeyIndex <= maxBoundary && newKeyIndex >= 0))
        {
            var newSquare = CurrentMapSquare(newKeyIndex);
            if (newSquare != null)
                targetSquares.Add(newSquare);
        }
    }
    private static bool IsOverlapped(int curIndex, int range, List<MapSquare> targetSquares)
    {
        if (curIndex < range)
        {
            if (targetSquares.Count > 0)
                return !targetSquares[^1].IsCanMove();
        }
        return false;
    }
#endregion
    public static int CurrentIndex(MapSquare curMapSquare)
    {
        if (curMapSquare == null)
            return -1;
        Vector2 curKey = _mapSquareDic.FirstOrDefault(x => x.Value == curMapSquare).Key;
        int curKeyIndexInt = _mapSquareDic.Keys.ToList().IndexOf(curKey);
        return curKeyIndexInt;
    }
    public static MapSquare CurrentMapSquare(int curKeyIndex)
    {
        //return _mapSquareDic.FirstOrDefault(x => x.Key == CurrentKey(curKeyIndex)).Value;
        return _mapSquareDic.Values.ToList()[curKeyIndex];
    }
    public static Vector2 CurrentKey(MapSquare curMapSquare)
    {
        return _mapSquareDic.FirstOrDefault(x => x.Value == curMapSquare).Key;
    }
    public static Vector2 CurrentKey(int curKeyIndex)
    {
        return _mapSquareDic.Keys.ToList()[curKeyIndex];
    }
}
