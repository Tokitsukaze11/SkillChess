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
    private const int TOWER_A_OBSTACLE = 0;
    private const int TOWER_B_OBSTACLE = 1;
    private const int TOWER_C_OBSTACLE = 2;
    private const int WALL_B_OBSTACLE = 3;
    private void Awake()
    {
        //ObjectManager.Instance.MakePool(_obstaclePrefabs[BOX_OBSTACLE], StringKeys.BOX_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[TOWER_A_OBSTACLE], StringKeys.TOWER_A_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[TOWER_B_OBSTACLE], StringKeys.TOWER_B_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[TOWER_C_OBSTACLE], StringKeys.TOWER_C_OBSTACLE);
        ObjectManager.Instance.MakePool(_obstaclePrefabs[WALL_B_OBSTACLE], StringKeys.WALL_B_OBSTACLE);
    }
    private string TypeString(int type)
    {
        switch (type)
        {
            case TOWER_A_OBSTACLE:
                return StringKeys.TOWER_A_OBSTACLE;
            case TOWER_B_OBSTACLE:
                return StringKeys.TOWER_B_OBSTACLE;
            case TOWER_C_OBSTACLE:
                return StringKeys.TOWER_C_OBSTACLE;
            case WALL_B_OBSTACLE:
                return StringKeys.WALL_B_OBSTACLE;
            default:
                return null;
        }
    }
    public void SpawnObstacle()
    {
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        // 1행의 마지막 열을 제외하고 모두 void 장애물이 있게 임시로 설정.
        for (int i = 0; i < col; i++)
        {
            if (i == col - 1)
                continue;
            int index = i * row + 1;
            VoidObstacle(index);
        }
        // 3행의 첫 열을 제외하고 모두 박스 장애물이 있게 임시로 설정.
        for (int i = 0; i < col; i++)
        {
            if (i == 0)
                continue;
            int index = i * row + 3;
            ObjectObstacle(index, Random.Range(0, _obstaclePrefabs.Length));
        }
    }
    private void VoidObstacle(int index) // 빈 공간
    {
        var targetMap = SquareCalculator.CurrentMapSquare(index);
        targetMap.IsObstacle = true;
        targetMap.gameObject.SetActive(false);
    }
    private void ObjectObstacle(int index, int type)
    {
        var targetMap = SquareCalculator.CurrentMapSquare(index);
        targetMap.IsObstacle = true;
        var obj = ObjectManager.Instance.SpawnObject(_obstaclePrefabs[type], TypeString(type), true);
        Vector3 targetPos = targetMap.transform.position;
        // TODO : 장애물의 위치가 각각 다를 수 있음(최대한 프리팹의 위치를 기준으로 잡아야 함)
        obj.transform.position += targetPos;
        obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
        obj.gameObject.SetActive(true);
    }
}
