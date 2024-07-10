using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("Debugging Spawn")]
    public GameObject place;
    public float squareSize = 1.5f;

    [ReadOnly] public int row = 8;
    [ReadOnly] public int col = 8;
    
    //private Dictionary<Vector2,MapSquare> _mapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    [SerializeField] private ObstacleSpawner _obstacleSpawner;
    
    private void Awake()
    {
        ObjectManager.Instance.MakePool(place, StringKeys.MAP_PLACE);
    }
    private void Start() // TODO : Will be called by other class
    {
        MakeMapSquares(row, col);
    }
    public void MakeMapSquares(int row, int col)
    {
        Dictionary<Vector2,MapSquare> mapSquareDic = new Dictionary<Vector2, MapSquare>();
        GlobalValues.ROW = row;
        GlobalValues.COL = col;
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Vector2 spawnPoint = new Vector2(i * squareSize, j * squareSize);
                var obj = ObjectManager.Instance.SpawnObject(place, StringKeys.MAP_PLACE, true);
                obj.transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);
                obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
                obj.gameObject.name = $"Place_{i}_{j}";
                mapSquareDic.Add(spawnPoint, obj.GetComponent<MapSquare>());
            }
        }
        PawnManager.Instance.SetMapSquareDic(mapSquareDic);
        PawnManager.Instance.SpawnPawn();
        _obstacleSpawner.SpawnObstacle();
        MoveNavigation.InitMapSquare(mapSquareDic);
    }
}
