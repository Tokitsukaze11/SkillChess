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
    public GameObject _player1HQ;
    public GameObject _player2HQ;
    private Vector3 _originPlayer1HQpos = new Vector3(5, -4.6f, -4.8f);
    private Vector3 _originPlayer2HQpos = new Vector3(5, -4.6f, 15.2f);

    [ReadOnly] private int row = 8;
    [ReadOnly] private int col = 8;
    
    //private Dictionary<Vector2,MapSquare> _mapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    [SerializeField] private ObstacleSpawner _obstacleSpawner;
    [SerializeField] private ObstacleController _obstacleController;
    
    private void Awake()
    {
        ObjectManager.Instance.MakePool(place, StringKeys.MAP_PLACE);
         /*_originPlayer1HQpos = _player1HQ.transform.position;
         _originPlayer2HQpos = _player2HQ.transform.position;*/
        GameManager.Instance.OnGameRestart += ResetMapSquares;
        EventManager.Instance.OnGameStart += MakeMapSquares;
        EventManager.Instance.OnTitle += ClearMapSquares;
        _obstacleSpawner.OnObstacleSet += _obstacleController.SetObstacle;
        GlobalValues.ROW = row;
        GlobalValues.COL = col;
    }
    private void MakeMapSquares(int row, int col)
    {
        GameManager.Instance.GameStateChange(GameState.Play);
        var targetX = (col - 8) * (squareSize / 2);
        var targetZ = (row - 8) * (squareSize);
        var newPos1 = new Vector3(_originPlayer1HQpos.x + targetX, _originPlayer1HQpos.y, _originPlayer1HQpos.z);
        var newPos2 = new Vector3(_originPlayer2HQpos.x + targetX, _originPlayer2HQpos.y, _originPlayer2HQpos.z + targetZ);
        _player1HQ.transform.position = newPos1;
        _player2HQ.transform.position = newPos2;
        Dictionary<Vector2,MapSquare> mapSquareDic = new Dictionary<Vector2, MapSquare>();
        //Debug.Log("행 : " + row + " 열 : " + col);
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Vector2 spawnPoint = new Vector2(i * squareSize, j * squareSize);
                var obj = ObjectManager.Instance.SpawnObject(place, StringKeys.MAP_PLACE, true);
                obj.transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);
                obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
                obj.gameObject.name = $"Place_{i}_{j}";
                var mapSquare = obj.GetComponent<MapSquare>();
                mapSquareDic.Add(spawnPoint, mapSquare);
            }
        }
        PawnManager.Instance.SetMapSquareDic(mapSquareDic);
        _obstacleSpawner.SpawnObstacle();
        NavMeshController.Instance.BakeNavMesh();
        PawnManager.Instance.SpawnPawn();
    }
    private void ResetMapSquares()
    {
        ClearMapSquares();
        MakeMapSquares(GlobalValues.ROW, GlobalValues.COL);
    }
    private void ClearMapSquares()
    {
        var mapSquareDic = SquareCalculator.MapSquareDic;
        foreach (var mapSquare in mapSquareDic.Values)
        {
            ObjectManager.Instance.RemoveObject(mapSquare.gameObject);
        }
        SquareCalculator.MapSquareDic.Clear();
        PawnManager.Instance.ResetPawns();
        _obstacleSpawner.InvisibleObstacle(); // Destroy시 Obstacle에서 SetAlpha를 호출하면서 문제 발생
    }
}
