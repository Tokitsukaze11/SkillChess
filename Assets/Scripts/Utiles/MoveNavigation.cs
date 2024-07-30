using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoveNavigation
{
    private static  List<List<int>> _convertedIndex = new List<List<int>>();
    private static Dictionary<Vector2, MapSquare> _convertedMapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    static bool[,] _obstacles; // 장애물 배열
    static int[,] _directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // 상하좌우 이동
    
    public static void InitMapSquare(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        _convertedIndex.Clear();
        _convertedMapSquareDic.Clear();
        Converter1DTo2D(mapSquareDic);
        bool[,] isObstacle = new bool[GlobalValues.ROW, GlobalValues.COL];
        for (int i = 0; i < GlobalValues.ROW; i++)
        {
            for (int j = 0; j < GlobalValues.COL; j++)
            {
                isObstacle[i, j] = false;
            }
        }
        foreach(var mapSquare in _convertedMapSquareDic.Values)
        {
            if (!mapSquare.IsObstacle)
                continue;
            int index = _convertedMapSquareDic.Values.ToList().IndexOf(mapSquare);
            int row = index / GlobalValues.COL;
            int col = index % GlobalValues.COL;
            isObstacle[row, col] = true;
        }
        _obstacles = isObstacle;
    }
    private static void Converter1DTo2D(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        // 행이 뒤집힘.
        
        int column = GlobalValues.COL; // 열
        int row = GlobalValues.ROW; // 행
        
        // 현재 1차원 배열은 좌하단을 시작으로 위로 올라가는 방식.
        // 2차원 배열로 변환할 때는 BFS를 쉽게 하기 위해서 좌상단을 시작으로 오른쪽으로 가는 방식으로 변환.
        // 먼저 원본을 무시한 채, 행과 열 개수만 신경 써서 2차원 배열을 만든다.
        int index = 0;
        for (int i = 0; i < row; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < column; j++)
            {
                tempList.Add(index++);
            }
            _convertedIndex.Add(tempList);
        }
        // 딕셔너리에 있던 MapSquare를 생성된 2차원 배열에 맞게 1차원 배열로 변환.
        var mapSquares = mapSquareDic.Values.ToList();
        List<MapSquare> tempMapSquares = new List<MapSquare>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                tempMapSquares.Add(mapSquares[i * column + j]);
            }
        }
        // 해당 1차원 배열을 바탕으로 딕셔너리로 다시 변환.
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var mapSquare = tempMapSquares[i * column + j];
                var key = mapSquareDic.FirstOrDefault(x => x.Value == mapSquare).Key;
                _convertedMapSquareDic.Add(key, mapSquare);
            }
        }
        // 이로 2차원 배열로 변환 완료.
    }
    public static Queue<MapSquare> FindReachablePositions(MapSquare startMapSquare, int range)
    {
        var mapList = _convertedMapSquareDic.Values.ToList();
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };
        
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        int startIndex = mapList.IndexOf(startMapSquare);

        bool[,] visited = new bool[row, col];
        int[,] distance = new int[row, col];

        Queue<int> queue = new Queue<int>();
        Queue<int> result = new Queue<int>();
        
        int startRow = startIndex / col;
        int startCol = startIndex % col;
        
        queue.Enqueue(startIndex);

        while (queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();
            int currentRow = currentIndex / col;
            int currentCol = currentIndex % col;
            
            if(distance[currentRow, currentCol] == range)
            {
                result.Enqueue(currentIndex);
                continue;
            }

            for (int i = 0; i < 4; i++)
            {
                int newRow = currentRow + dx[i];
                int newCol = currentCol + dy[i];
                
                if(IsValid(newRow,newCol) && !visited[newRow,newCol] && !_obstacles[newRow,newCol])
                {
                    queue.Enqueue(newRow * col + newCol);
                    visited[newRow, newCol] = true;
                    distance[newRow, newCol] = distance[currentRow, currentCol] + 1;
                }
            }
        }
        Queue<MapSquare> resultSquares = new Queue<MapSquare>();
        foreach (var index in result)
        {
            resultSquares.Enqueue(mapList[index]);
        }
        return resultSquares;
    }
    public static Queue<MapSquare> FindNavigation(MapSquare start, MapSquare end)
    {
        var mapList = _convertedMapSquareDic.Values.ToList();
        int startIndex = mapList.IndexOf(start);
        int endIndex = mapList.IndexOf(end);
        
        Queue<int> path = FindShortestPath(startIndex, endIndex);
        if(path.Count == 0)
        {
            return null;
        }
        Queue<MapSquare> pathMapSquare = new Queue<MapSquare>();
        foreach (var index in path)
        {
            pathMapSquare.Enqueue(mapList[index]);
        }
        return pathMapSquare;
    }
    private static Queue<int> FindShortestPath(int startIndex, int endIndex, bool isForwardTracking = true)
    {
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        bool[][] visited = new bool[row][];
        for (int index = 0; index < row; index++)
        {
            visited[index] = new bool[col];
        }
        int[,] parent = new int[row, col];
        Queue<int> queue = new Queue<int>();

        int startRow = startIndex / col;
        int startCol = startIndex % col;
        int endRow = endIndex / col;
        int endCol = endIndex % col;

        queue.Enqueue(startIndex);
        visited[startRow][startCol] = true;
        for (int i = 0; i < row; i++)
            for (int j = 0; j < col; j++)
                parent[i, j] = -1;

        while (queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();
            int currentRow = currentIndex / col;
            int currentCol = currentIndex % col;

            if (currentIndex == endIndex)
            {
                return ReconstructPath(parent, startIndex, endIndex, isForwardTracking);
            }

            for (int i = 0; i < 4; i++)
            {
                int newRow = currentRow + _directions[i, 0];
                int newCol = currentCol + _directions[i, 1];
                int newIndex = newRow * col + newCol;

                if (IsValid(newRow, newCol) && !visited[newRow][newCol] && !_obstacles[newRow, newCol])
                {
                    queue.Enqueue(newIndex);
                    visited[newRow][newCol] = true;
                    parent[newRow, newCol] = currentIndex;
                }
            }
        }

        return new Queue<int>(); // 경로를 찾지 못한 경우
    }
    private static Queue<int> ReconstructPath(int[,] parent, int startIndex, int endIndex, bool isForwardTracking)
    {
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        Queue<int> path = new Queue<int>();

        if (isForwardTracking)
        {
            // 정방향 추적
            Stack<int> stack = new Stack<int>();
            int current = endIndex;
            while (current != -1)
            {
                stack.Push(current);
                int parentRow = current / col;
                int parentCol = current % col;
                current = parent[parentRow, parentCol];
            }
            while (stack.Count > 0)
            {
                path.Enqueue(stack.Pop());
            }
        }
        else
        {
            // 역방향 추적
            int current = endIndex;
            while (current != -1)
            {
                path.Enqueue(current);
                int parentRow = current / col;
                int parentCol = current % col;
                current = parent[parentRow, parentCol];
            }
        }

        return path;
    }
    private static bool IsValid(int row, int col)
    {
        return row >= 0 && row < GlobalValues.ROW && col >= 0 && col < GlobalValues.COL;
    }
}
