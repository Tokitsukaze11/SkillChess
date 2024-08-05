using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public GameObject place;
    public float squareSize = 1.5f;
    public GameObject _player2HQ;
    private Vector3 _originPlayer2HQpos;

    [ReadOnly] public int row = 8;
    [ReadOnly] public int col = 8;
    
    //private Dictionary<Vector2,MapSquare> _mapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    [SerializeField] private ObstacleSpawner _obstacleSpawner;
    
    private void Awake()
    {
        ObjectManager.Instance.MakePool(place, StringKeys.MAP_PLACE);
        _originPlayer2HQpos = _player2HQ.transform.position;
        GameManager.Instance.OnGameRestart += ResetMapSquares;
    }
    private void Start() // TODO : Will be called by other class
    {
        GlobalValues.ROW = row;
        GlobalValues.COL = col;
        MakeMapSquares(row, col);
    }
    private void MakeMapSquares(int row, int col)
    {
        _originPlayer2HQpos.z += (row - 8) * squareSize;
        _player2HQ.transform.position = _originPlayer2HQpos;
        Dictionary<Vector2,MapSquare> mapSquareDic = new Dictionary<Vector2, MapSquare>();
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
        _obstacleSpawner.SpawnObstacle();
        NavMeshController.Instance.BakeNavMesh();
        PawnManager.Instance.SpawnPawn();
    }
    public void ResetMapSquares()
    {
        var mapSquareDic = SquareCalculator.MapSquareDic;
        foreach (var mapSquare in mapSquareDic.Values)
        {
            ObjectManager.Instance.RemoveObject(mapSquare.gameObject);
        }
        SquareCalculator.MapSquareDic.Clear();
        PawnManager.Instance.ResetPawns();
        MakeMapSquares(GlobalValues.ROW, GlobalValues.COL);
    }
}
