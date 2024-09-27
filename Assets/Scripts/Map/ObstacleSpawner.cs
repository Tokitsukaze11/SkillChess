using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle")]
    [SerializeField] GameObject[] _obstaclePrefabs;
    private List<GameObject> _obstacleList = new List<GameObject>();
    private const int TOWER_A_OBSTACLE = 0;
    private const int TOWER_B_OBSTACLE = 1;
    private const int TOWER_C_OBSTACLE = 2;
    private const int WALL_B_OBSTACLE = 3;
    public event Action<List<Obstacle>> OnObstacleSet;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_obstaclePrefabs[TOWER_A_OBSTACLE], StringKeys.TOWER_A_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[TOWER_B_OBSTACLE], StringKeys.TOWER_B_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[TOWER_C_OBSTACLE], StringKeys.TOWER_C_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[WALL_B_OBSTACLE], StringKeys.WALL_B_OBSTACLE);
    }
    private string TypeString(int type)
    {
        return type switch
        {
            TOWER_A_OBSTACLE => StringKeys.TOWER_A_OBSTACLE,
            TOWER_B_OBSTACLE => StringKeys.TOWER_B_OBSTACLE,
            TOWER_C_OBSTACLE => StringKeys.TOWER_C_OBSTACLE,
            WALL_B_OBSTACLE => StringKeys.WALL_B_OBSTACLE,
            _ => null
        };
    }
    public void InvisibleObstacle()
    {
        foreach (var obstacle in _obstacleList)
        {
            obstacle.gameObject.SetActive(false);
        }
    }
    private void ResetObstacle()
    {
        if(_obstacleList.Count > 0)
            foreach(var obstacle in _obstacleList)
                ObjectManager.Instance.RemoveObject(obstacle);
        _obstacleList.Clear();
    }
    public void SpawnObstacle()
    {
        ResetObstacle();
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        List<int> obstacleList = new List<int>();
        
        // 각각 아래 2개행과 위 2개행은 장애물이 없게 설정.
        // 다른 모든 곳은 장애물을 랜덤으로 생성.
        // BFS로 경로가 존재하는지 확인하도록 만들기.
        do
        {
            obstacleList.Clear();
            for (int i = 0; i < row*col; i++)
            {
                int index = i;
                SquareCalculator.CurrentMapSquare(index).IsObstacle = false; // 장애물 초기화
                int curRow = i % row;
                if (curRow < 2 || curRow >= row - 2)
                    continue;
                int rand = Random.Range(0, 100);
                if(rand < 50)
                    continue;
                SquareCalculator.CurrentMapSquare(index).IsObstacle = true; // 코드 상으로만 장애물 생성
                obstacleList.Add(index);
            }
            MoveNavigation.InitMapSquare(SquareCalculator.MapSquareDic);
        } while (MoveNavigation.FindNavigation(SquareCalculator.CurrentMapSquare(0), SquareCalculator.CurrentMapSquare(col * row - 1)) == null);
        foreach (int map in obstacleList) // 실제 장애물 생성
        {
            int randType = Random.Range(0, _obstaclePrefabs.Length + 1);
            if (randType == _obstaclePrefabs.Length)
                VoidObstacle(map);
            else
                _obstacleList.Add(ObjectObstacle(map, randType));
        }
        OnObstacleSet?.Invoke(_obstacleList.Select(x => x.GetComponent<Obstacle>()).ToList());
    }
    private void VoidObstacle(int index) // 빈 공간
    {
        var targetMap = SquareCalculator.CurrentMapSquare(index);
        targetMap.IsObstacle = true;
        targetMap.gameObject.SetActive(false);
    }
    private GameObject ObjectObstacle(int index, int type)
    {
        var targetMap = SquareCalculator.CurrentMapSquare(index);
        targetMap.IsObstacle = true;
        var obj = ObjectManager.Instance.SpawnObject(_obstaclePrefabs[type], TypeString(type), true);
        Vector3 targetPos = targetMap.transform.position;
        // 장애물의 위치가 각각 다를 수 있음(최대한 프리팹의 위치를 기준으로 잡아야 함)
        obj.transform.position += targetPos;
        obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
        obj.gameObject.SetActive(true);
        targetMap.Obstacle = obj.GetComponent<Obstacle>();
        return obj;
    }
}
