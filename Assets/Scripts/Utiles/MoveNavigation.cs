using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveNavigation : MonoBehaviour
{
    private List<List<int>> _convertedIndex = new List<List<int>>();
    private Dictionary<Vector2, MapSquare> _convertedMapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    public void InitMapSquare(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        _convertedIndex.Clear();
        _convertedMapSquareDic.Clear();
        Converter1DTo2D(mapSquareDic);
    }
    private void Converter1DTo2D(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        // 행이 뒤집힘.
        
        int column = GlobalValues.COL; // 열
        int row = GlobalValues.ROW; // 행
        
        // 현재 1차원 배열은 좌하단을 시작으로 위로 올라가는 방식.
        // 2차원 배열로 변환할 때는 BFS를 쉽게 하기 위해서 좌상단을 시작으로 오른쪽으로 가는 방식으로 변환.
        // 먼저 원본을 무시한 채, 행과 열 개수만 신경 써서 2차원 배열을 만든다.
        for (int i = 0; i < row; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < column; j++)
            {
                tempList.Add(0);
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
    private void BFS(MapSquare start, MapSquare end)
    {
        int startIndex = _convertedMapSquareDic.Values.ToList().IndexOf(start);
        int endIndex = _convertedMapSquareDic.Values.ToList().IndexOf(end);
        // _convertedIndex를 바탕으로 BFS를 진행. 단, _convertedIndex는 장애물의 존재 여부를 포함하지 않음. 또한, _convertedIndex를 수정하지 않고 BFS를 진행해야 함.
        // 장애물의 존재 여부를 확인하는 방법은 MapSquare의 !IsObstacle()을 통해 확인. 단, IsObstacle()은 현재 false만 반환 중. => 장애물 존재 안한다고 가정.
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
            if (!mapSquare.IsObstacle())
                continue;
            int index = _convertedMapSquareDic.Values.ToList().IndexOf(mapSquare);
            int row = index / GlobalValues.COL;
            int col = index % GlobalValues.COL;
            isObstacle[row, col] = true;
        }
        // BFS 진행
        bool[,] visited = new bool[GlobalValues.ROW, GlobalValues.COL];
        for (int i = 0; i < GlobalValues.ROW; i++)
        {
            for (int j = 0; j < GlobalValues.COL; j++)
            {
                visited[i, j] = false;
            }
        }
        int[] dx = {0, 0, 1, -1}; // 행렬 중 x값 => 열
        int[] dy = {1, -1, 0, 0}; // 행렬 중 y값 => 행
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(startIndex);
        visited[startIndex / GlobalValues.COL, startIndex % GlobalValues.COL] = true;
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
    }
}
