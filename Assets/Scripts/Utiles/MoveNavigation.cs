using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoveNavigation
{
    private static  List<List<int>> _convertedIndex = new List<List<int>>();
    private static Dictionary<Vector2, MapSquare> _convertedMapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    public static void InitMapSquare(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        _convertedIndex.Clear();
        _convertedMapSquareDic.Clear();
        Converter1DTo2D(mapSquareDic);
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
    public static void FindNavigation(MapSquare start, MapSquare end)
    {
        var mapList = _convertedMapSquareDic.Values.ToList();
        int startIndex = mapList.IndexOf(start);
        int endIndex = mapList.IndexOf(end);
        // _convertedIndex를 바탕으로 BFS를 진행. 단, _convertedIndex는 장애물의 존재 여부를 포함하지 않음. 또한, _convertedIndex를 수정하지 않고 BFS를 진행해야 함.
        // 장애물의 존재 여부를 확인하는 방법은 MapSquare의 !IsObstacle을 통해 확인.
        // 장애물의 존재 여부를 포함하는 배열을 만들어야 함.
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
        // 쉬운 진행을 위해 _convertedIndex를 바탕으로 행렬을 만듦.
        List<List<int>> matrix = new List<List<int>>(_convertedIndex);
        // 위 행렬을 기준으로 BFS를 진행.
        bool[,] visited = new bool[GlobalValues.ROW, GlobalValues.COL];
        for (int i = 0; i < GlobalValues.ROW; i++)
        {
            for (int j = 0; j < GlobalValues.COL; j++)
            {
                visited[i, j] = false;
            }
        }
        int[] dx = {0, -1, 0, 1}; // 행렬 중 x값 => 열
        int[] dy = {-1, 0, 1, 0}; // 행렬 중 y값 => 행
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(startIndex);
        var curIndex = matrix.FirstOrDefault(x => x.Contains(startIndex));
        if (curIndex != null)
            visited[curIndex.IndexOf(startIndex), curIndex.IndexOf(startIndex)] = true;
        while (queue.Count > 0)
        {
            int now = queue.Dequeue();
            if (now == endIndex)
                break;
            int nowRow = now / GlobalValues.COL;
            int nowCol = now % GlobalValues.COL;
            for (int i = 0; i < 4; i++)
            {
                int nextRow = nowRow + dy[i];
                int nextCol = nowCol + dx[i];
                if (nextRow < 0 || nextRow >= GlobalValues.ROW || nextCol < 0 || nextCol >= GlobalValues.COL)
                    continue;
                if (visited[nextRow, nextCol] || isObstacle[nextRow, nextCol])
                    continue;
                visited[nextRow, nextCol] = true;
                queue.Enqueue(nextRow * GlobalValues.COL + nextCol);
            }
        }
        // BFS 종료
        // TODO : 정상적으로 작동하는지 확인 필요.
        // 정상 작동을 확인하기 위해 이동 경로의 MapSquare의 색을 하얀색으로 바꿈
        
    }
}
