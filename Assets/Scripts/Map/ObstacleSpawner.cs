using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle")]
    [SerializeField] GameObject[] _obstaclePrefabs;
    private const int BOX_OBSTACLE = 0;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_obstaclePrefabs[BOX_OBSTACLE], StringKeys.BOX_OBSTACLE);
    }
    private string TypeString(int type)
    {
        switch (type)
        {
            case BOX_OBSTACLE:
                return StringKeys.BOX_OBSTACLE;
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
            ObjectObstacle(index, BOX_OBSTACLE);
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
        targetPos.y += 0.5f;
        obj.transform.position = targetPos;
        obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
        obj.gameObject.SetActive(true);
    }
}
