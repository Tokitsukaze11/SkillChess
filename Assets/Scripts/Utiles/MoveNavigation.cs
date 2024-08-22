using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
 * ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!ㅅㅂ 건들지마!! ㅅㅂ 건들지마!
*/
public static class MoveNavigation
{
    private static  List<List<int>> _convertedIndex = new List<List<int>>();
    private static Dictionary<Vector2, MapSquare> _convertedMapSquareDic = new Dictionary<Vector2, MapSquare>();
    private static List<MapSquare> _convertedMapSquareList = new List<MapSquare>();
    
    static bool[,] _obstacles; // 장애물 배열
    static int[,] _directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // 상하좌우 이동
    
    public static void InitMapSquare(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        _convertedIndex.Clear();
        _convertedMapSquareDic.Clear();
        _convertedMapSquareList.Clear();
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
        /*for (int i = row-1; i >= 0; i--)
        {
            for (int j = 0; j < column; j++)
            {
                tempMapSquares.Add(mapSquares[i * column + j]);
                Debug.Log("Now Index : " + tempMapSquares.IndexOf(mapSquares[i * column + j]) + " origin Index : " + (i * column + j));
            }
        }*/
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                int originIndex = (row-1-i)+j * row;
                tempMapSquares.Add(mapSquares[originIndex]);
                //Debug.Log("Now Index : " + tempMapSquares.IndexOf(mapSquares[originIndex]) + " origin Index : " + originIndex);
            }
        }
        // 해당 1차원 배열을 바탕으로 딕셔너리로 다시 변환.
        /*for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var mapSquare = tempMapSquares[i * column + j];
                var key = mapSquareDic.FirstOrDefault(x => x.Value == mapSquare).Key;
                _convertedMapSquareDic.Add(key, mapSquare);
            }
        }*/
        foreach(var mapSquare in tempMapSquares)
        {
            var key = mapSquareDic.FirstOrDefault(x => x.Value == mapSquare).Key;
            _convertedMapSquareDic.Add(key, mapSquare);
        }
        // 굳이 딕셔너리를 사용할 필요는 없음. 키를 쓸일이 없음. -> 리스트로 변환
        foreach(var square in tempMapSquares)
            _convertedMapSquareList.Add(square);
        /*foreach(var map in _convertedMapSquareList)
            Debug.Log("Now Index : " + _convertedMapSquareList.IndexOf(map) + " origin Index : " + SquareCalculator.CurrentIndex(map));*/
        // 이로 2차원 배열로 변환 완료.
    }
    public static Queue<MapSquare> FindReachablePositions(MapSquare startMapSquare, int range)
    {
        /*var mapList = _convertedMapSquareDic.Values.ToList();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        
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
            
            if(distance[currentRow, currentCol] <= range)
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
        return resultSquares;*/
        //var mapList = _convertedMapSquareDic.Values.ToList();
        var mapList = _convertedMapSquareList;
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
    
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
    
        //int startIndex = mapList.IndexOf(startMapSquare);
        int startIndex = _convertedMapSquareList.IndexOf(startMapSquare);

        bool[,] visited = new bool[row, col];
        int[,] distance = new int[row, col];

        Queue<int> queue = new Queue<int>();
        Queue<MapSquare> resultSquares = new Queue<MapSquare>();
    
        int startRow = startIndex / col;
        int startCol = startIndex % col;
        
        //Debug.Log($"StartRow : {startRow}, StartCol : {startCol}, StartIndex : {startIndex}");
    
        queue.Enqueue(startIndex);
        visited[startRow, startCol] = true;

        while (queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();
            int currentRow = currentIndex / col;
            int currentCol = currentIndex % col;
        
            if(distance[currentRow, currentCol] <= range)
            {
                resultSquares.Enqueue(mapList[currentIndex]);

                for (int i = 0; i < 4; i++)
                {
                    int newRow = currentRow + dx[i];
                    int newCol = currentCol + dy[i];
                
                    if (IsValid(newRow, newCol) && !visited[newRow, newCol] && !_obstacles[newRow, newCol])
                    {
                        int newIndex = newRow * col + newCol;
                        queue.Enqueue(newIndex);
                        visited[newRow, newCol] = true;
                        distance[newRow, newCol] = distance[currentRow, currentCol] + 1;
                    }
                }
            }
        }
        return resultSquares;
        /*var mapList = _convertedMapSquareDic.Values.ToList();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;

        int startIndex = mapList.IndexOf(startMapSquare);

        bool[,] visited = new bool[row, col];
        int[,] distance = new int[row, col];

        Queue<int> queue = new Queue<int>();
        Queue<MapSquare> resultSquares = new Queue<MapSquare>();

        int startRow = startIndex / col;
        int startCol = startIndex % col;

        queue.Enqueue(startIndex);
        visited[startRow, startCol] = true;
        distance[startRow, startCol] = 0;  // 시작 위치의 거리를 0으로 초기화

        while (queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();
            int currentRow = currentIndex / col;
            int currentCol = currentIndex % col;
        
            if(distance[currentRow, currentCol] <= range)
            {
                resultSquares.Enqueue(mapList[currentIndex]);

                for (int i = 0; i < 4; i++)
                {
                    int newRow = currentRow + dx[i];
                    int newCol = currentCol + dy[i];
                
                    if (IsValid(newRow, newCol) && !visited[newRow, newCol] && !_obstacles[newRow, newCol])
                    {
                        int newIndex = newRow * col + newCol;
                        int newDistance = distance[currentRow, currentCol] + 1;
                    
                        if (newDistance <= range)  // 새로운 위치의 거리가 range 이하인 경우에만 큐에 추가
                        {
                            queue.Enqueue(newIndex);
                            visited[newRow, newCol] = true;
                            distance[newRow, newCol] = newDistance;
                        }
                    }
                }
            }
        }
    
        return resultSquares;*/
        /*var mapList = _convertedMapSquareDic.Values.ToList();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;

        int startIndex = mapList.IndexOf(startMapSquare);

        bool[,] visited = new bool[row, col];
        int[,] distance = new int[row, col];

        Queue<(int, int)> queue = new Queue<(int, int)>();
        Queue<MapSquare> resultSquares = new Queue<MapSquare>();

        int startRow = startIndex / col;
        int startCol = startIndex % col;
        
        Debug.Log($"StartRow : {startRow}, StartCol : {startCol}, StartIndex : {startIndex}");

        queue.Enqueue((startRow, startCol));
        visited[startRow, startCol] = true;
        distance[startRow, startCol] = 0;

        while (queue.Count > 0)
        {
            var (currentRow, currentCol) = queue.Dequeue();
        
            if(distance[currentRow, currentCol] <= range)
            {
                resultSquares.Enqueue(mapList[currentRow * col + currentCol]);

                for (int i = 0; i < 4; i++)
                {
                    int newRow = currentRow + dx[i];
                    int newCol = currentCol + dy[i];
                
                    if (IsValid(newRow, newCol) && !visited[newRow, newCol] && !_obstacles[newRow, newCol])
                    {
                        int newDistance = distance[currentRow, currentCol] + 1;
                    
                        if (newDistance <= range)
                        {
                            queue.Enqueue((newRow, newCol));
                            visited[newRow, newCol] = true;
                            distance[newRow, newCol] = newDistance;
                        }
                    }
                }
            }
        }
        return resultSquares;*/
        /*var mapList = _convertedMapSquareDic.Values.ToList();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;

        int startIndex = mapList.IndexOf(startMapSquare);

        bool[,] visited = new bool[row, col];
        Queue<MapSquare> resultSquares = new Queue<MapSquare>();

        int startRow = startIndex % col;
        int startCol = startIndex / col;
        
        Debug.Log($"StartRow : {startRow}, StartCol : {startCol}, StartIndex : {startIndex}");

        DFS(startRow, startCol, 0, range, visited, resultSquares, mapList, row, col);

        return resultSquares;*/
    }
    private static void DFS(int currentRow, int currentCol, int currentDistance, int maxRange, 
                            bool[,] visited, Queue<MapSquare> resultSquares, List<MapSquare> mapList, 
                            int totalRows, int totalCols)
    {
        if (currentDistance > maxRange || !IsValid(currentRow, currentCol) || 
            visited[currentRow, currentCol] || _obstacles[currentRow, currentCol])
        {
            return;
        }

        visited[currentRow, currentCol] = true;
        resultSquares.Enqueue(mapList[currentRow * totalCols + currentCol]);

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newRow = currentRow + dx[i];
            int newCol = currentCol + dy[i];

            DFS(newRow, newCol, currentDistance + 1, maxRange, visited, resultSquares, mapList, totalRows, totalCols);
        }
    }
    public static Queue<MapSquare> FindNavigation(MapSquare start, MapSquare end)
    {
        //var mapList = _convertedMapSquareDic.Values.ToList();
        var mapList = _convertedMapSquareList;
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
