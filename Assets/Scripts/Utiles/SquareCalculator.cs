using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public static class SquareCalculator
{
    private static Dictionary<Vector2, MapSquare> _mapSquareDic;
    public static Dictionary<Vector2, MapSquare> MapSquareDic
    {
        set
        {
            _mapSquareDic = value;
        }
        get => _mapSquareDic;
    }
    #region Check Target Squares
    /// <summary>
    /// Check squares that can something be done as range
    /// </summary>
    /// <param name="targetRange">Range of do something</param>
    /// <param name="startSquare">Start square</param>
    /// <param name="targetSquares">List of target squares</param>
    /// <param name="isCanLess">Is can less of range</param>
    public static void CheckTargetSquaresAsRange(int targetRange, MapSquare startSquare, List<MapSquare> targetSquares, bool isCanLess = false)
    {
        if (!isCanLess)
        {
            Queue<MapSquare> target = MoveNavigation.FindReachablePositions(startSquare, targetRange);
            foreach (var map in target)
            {
                targetSquares.Add(map);
            }
        }
        else
        {
            HashSet<MapSquare> mapSet = new HashSet<MapSquare>();
            List<Queue<MapSquare>> target = new List<Queue<MapSquare>>();
            for (int i = targetRange; i > 0; i--)
            {
                int range = i;
                /*target.Add(MoveNavigation.FindReachablePositions(startSquare, range));*/
                var targetSquaresList = MoveNavigation.FindReachablePositions(startSquare, range);
                foreach (var destination in targetSquaresList)
                {
                    var path = MoveNavigation.FindNavigation(startSquare, destination);
                    foreach (var sq in path)
                        mapSet.Add(sq);
                }
            }
            /*foreach (var destinations in target)
            {
                foreach(var destination in destinations)
                {
                    var path = MoveNavigation.FindNavigation(startSquare, destination);
                    foreach (var sq in path)
                        mapSet.Add(sq);
                }
            }*/
            //targetSquares = mapSet.ToList();
            foreach (var map in mapSet)
            {
                targetSquares.Add(map);
            }
        }
        //var target = MoveNavigation.FindReachablePositions(startSquare, targetRange);
        /*if(!isCanLess)
        {
            foreach (var map in target)
            {
                targetSquares.Add(map);
            }
        }
        else
        {
            Queue<MapSquare> targets = new Queue<MapSquare>();
            foreach (var map in target.Select(sq => MoveNavigation.FindNavigation(startSquare, sq)).SelectMany(path => path))
            {
                targets.Enqueue(map);
            }
            foreach (var map in targets)
            {
                targetSquares.Add(map);
            }
        }*/
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
        GetVerticalCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles, isConsideringAnyPawn);
        GetHorizontalCheck(targetRange, curKeyIndex, targetSquares, isConsideringObstacles, isConsideringAnyPawn);
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
                
                if(CheckBool(isConsideringObstacles, false, targetSquares))
                    break;
            }
        }
    }
    private static void GetVerticalCheck(int targetRange, int curKeyIndex, List<MapSquare> targetSquares, bool isConsideringObstacles = false, bool isConsideringAnyPawn = false)
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
            int curRange = 0;
            while (curRange < targetRange)
            {
                int newKeyIndex = curKeyIndex + (curRange + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
                
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
            int curRange = 0;
            while (curRange < targetRange)
            {
                int newKeyIndex = curKeyIndex + (curRange + 1) * direction;
                CheckDirection(newKeyIndex, minBoundary, maxBoundary, _mapSquareDic.Keys.ToList(), targetSquares);
                
                if(CheckBool(isConsideringObstacles, isConsideringAnyPawn, targetSquares))
                    break;
                
                curRange++;
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
    private static bool CheckBool(bool isConsideringObstacles, bool isConsideringAnyPawn, List<MapSquare> targetSquares)
    {
        if (isConsideringObstacles && isConsideringAnyPawn) // 타겟 지정이고 장애물 영향 있을 때
        {
            if (targetSquares.Any(x => x.IsAnyPawn()))
                return true;
            if (targetSquares.Any(x => x.IsObstacle))
            {
                targetSquares.RemoveAt(targetSquares.Count - 1);
                return true;
            }
        }
        if (isConsideringAnyPawn && targetSquares.Any(x => x.IsAnyPawn())) // 타겟 지정이고 타겟 영향 있을 때
        {
            /*if(targetSquares[^1].CurPawn!._isPlayerPawn)
                targetSquares.RemoveAt(targetSquares.Count - 1);*/
            if (targetSquares[^1].CurPawn == null)
                return false;
            if(targetSquares[^1].CurPawn!._isPlayerPawn)
                targetSquares.RemoveAt(targetSquares.Count - 1);
            return true;
        }
        if (isConsideringObstacles && targetSquares.Any(x =>x.IsObstacle)) // 장애물 영향 있고 타겟 영향 없을 때
        {
            targetSquares.RemoveAt(targetSquares.Count - 1);
            return true;
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
    [Obsolete("Use CurrentMapSquare(int curKeyIndex) instead. Don't suggest find current map square by key.")]
    public static MapSquare CurrentMapSquare(Vector2 curKey)
    {
        return _mapSquareDic.Keys.ToList().Where(x => Mathf.Approximately(x.x, curKey.x) && Mathf.Approximately(x.y, curKey.y)).Select(x => _mapSquareDic[x]).FirstOrDefault();
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
