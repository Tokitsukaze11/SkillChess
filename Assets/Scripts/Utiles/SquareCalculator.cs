using System;
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
            _mapSquares = _mapSquareDic.Values.ToList();
            _keys = _mapSquareDic.Keys.ToList();
        }
        get => _mapSquareDic;
    }
    private static List<MapSquare> _mapSquares;
    private static List<Vector2> _keys;
    #region Check Target Squares
    /// <summary>
    /// Check squares that can something be done as range
    /// </summary>
    /// <param name="targetRange">Range of do something</param>
    /// <param name="startSquare">Start square</param>
    /// <param name="targetSquares">List of target squares</param>
    public static void CheckTargetSquaresAsRange(int targetRange, MapSquare startSquare, List<MapSquare> targetSquares)
    {
        HashSet<MapSquare> mapSet = new HashSet<MapSquare>();
        int index = CurrentIndex(startSquare);
        int startRow = index % GlobalValues.ROW;
        int startCol = index / GlobalValues.ROW;
        var targetSquaresList = MoveNavigation.FindReachablePositions(startSquare, targetRange);
        foreach (var destination in targetSquaresList)
        {
            var path = MoveNavigation.FindNavigation(startSquare, destination);
            foreach (var sq in path)
                mapSet.Add(sq);
        }
        targetSquares.AddRange(mapSet);
    }
    /// <summary>
    /// Check squares that can something be done
    /// </summary>
    /// <param name="targetRange">Range of do something</param>
    /// <param name="curKeyIndex">Current key index</param>
    /// <param name="targetSquares">List of target squares</param>
    /// <param name="isConsideringObstacles">장애물 영향을 받는지</param>
    /// <param name="isConsideringAnyPawn">기물 영향을 받는지(타겟 지정일 시)</param>
    public static void CheckTargetSquares(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false, bool isConsideringAnyPawn = false)
    {
        GetDirectionCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles, isConsideringAnyPawn);
        GetDirectionCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles, isConsideringAnyPawn, false);
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
        //var keys = _mapSquareDic.Keys.ToList();
        int row = GlobalValues.ROW; // n
        int col = GlobalValues.COL; // m
        
        int nowRow = curKeyIndex % row;
        int nowCol = curKeyIndex / row;
        
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
                CheckDirection(newKeyIndex, 0, row * col - 1, _keys, targetSquares);
                
                if(CheckBool(isConsideringObstacles, false, targetSquares))
                    break;
            }
        }
    }
    #region Check Vertical and Horizontal as isolated
    private static void GetVerticalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false, bool isConsideringAnyPawn = false)
    {
        int row = GlobalValues.ROW; // n
        int col = GlobalValues.COL; // m
        
        int nowCol = curKeyIndex / row;
        
        int maxBoundary = row * (nowCol + 1) - 1;
        int minBoundary = row * nowCol;
        
        int[] directions = { 1, -1 }; // 위, 아래 방향

        foreach (int direction in directions)
        {
            int curRange = 0;
            while (curRange < targetRange)
            {
                int newKeyIndex = curKeyIndex + (curRange + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _keys, targetSquares);
                
                if(CheckBool(isConsideringObstacles, isConsideringAnyPawn, targetSquares))
                    break;

                curRange++;
            }
        }
        // n*m (n 행 m 열)
        // 나의 열에서의 최대 값 : n*(현재 열+1)-1
        // 나의 열에서의 최소 값 : n*현재 열
    }
    private static void GetHorizontalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false, bool isConsideringAnyPawn = false)
    {
        int row = GlobalValues.ROW; // n
        int col = GlobalValues.COL; // m

        int nowRow = curKeyIndex % row;
        
        int minBoundary = nowRow;
        int maxBoundary = curKeyIndex + (row * targetRange);
        
        int[] directions = { row, -row }; // 오른쪽, 왼쪽 방향

        foreach (int direction in directions)
        {
            int curRange = 0;
            while (curRange < targetRange)
            {
                int newKeyIndex = curKeyIndex + (curRange + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _keys, targetSquares);
                
                if(CheckBool(isConsideringObstacles, isConsideringAnyPawn, targetSquares))
                    break;
                
                curRange++;
            }
        }
        // n*m (n 행 m 열)
        // 나의 행에서 이동할 수 있는 거리만큼의 최대 값 : 현재 인덱스 + (n*이동할 수 있는 거리)
        // 나의 행에서 이동할 수 있는 거리만큼의 최소 값 : 현재 행의 인덱스
    }
  #endregion
    // Merge GetVerticalCheck and GetHorizontalCheck
    private static void GetDirectionCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false, bool isConsideringAnyPawn = false, bool isVertical = true)
    {
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        int nowCol = curKeyIndex / row;
        int nowRow = curKeyIndex % row;

        int maxBoundary = isVertical ? row * (nowCol + 1) - 1 : curKeyIndex + (row * targetRange);
        int minBoundary = isVertical ? row * nowCol : nowRow;
        
        int[] directions = isVertical ? new int[] { 1, -1 } : new int[] { row, -row };
        
        foreach (int direction in directions)
        {
            int curRange = 0;
            while (curRange < targetRange)
            {
                int newKeyIndex = curKeyIndex + (curRange + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _keys, targetSquares);
                
                if(CheckBool(isConsideringObstacles, isConsideringAnyPawn, targetSquares))
                    break;

                curRange++;
            }
        }
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
    private static bool CheckBool(bool isConsideringObstacles, bool isConsideringAnyPawn, List<MapSquare> targetSquares)
    {
        switch (isConsideringObstacles, isConsideringAnyPawn, targetSquares)
        {
            case (true,true,var squares) when squares.Any(x=>x.IsAnyPawn()):
                return true;
            case (true,true,var squares) when squares.Any(x=>x.IsObstacle):
                targetSquares.RemoveAt(targetSquares.Count - 1);
                return true;
            case (true,false,var squares) when squares.Any(x=>x.IsObstacle):
                targetSquares.RemoveAt(targetSquares.Count - 1);
                return true;
            case (false,true,var squares)
                when squares.Any(x=>x.IsAnyPawn()) && !squares[^1].CurPawn.Equals(null):
                if (!squares[^1].CurPawn._isPlayerPawn)
                    return false;
                targetSquares.RemoveAt(targetSquares.Count - 1);
                return true;
            default:
                return false;
        }
    }
#endregion
    public static int CurrentIndex(MapSquare curMapSquare)
    {
        if (curMapSquare == null)
            return -1;
        Vector2 curKey = _mapSquareDic.FirstOrDefault(x => x.Value == curMapSquare).Key;
        int curKeyIndexInt = _keys.IndexOf(curKey);
        return curKeyIndexInt;
    }
    public static MapSquare CurrentMapSquare(int curKeyIndex)
    {
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
