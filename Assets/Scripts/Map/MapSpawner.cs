using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("Debugging Spawn")]
    public GameObject place;
    
    private Vector2[] _square = new Vector2[64]; // x: 행, z: 열
    private MapSquare[,] _mapSquares = new MapSquare[8, 8];
    private Dictionary<Vector2,MapSquare> _mapSquareDic = new Dictionary<Vector2, MapSquare>();
    
    private void Awake()
    {
        ObjectManager.Instance.MakePool(place, StringKeys.MAP_PLACE);
    }
    private void Start() // TODO : Will be called by other class
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                /*GameObject go = Instantiate(place, new Vector3(i*1.5f, 0, j*1.5f), Quaternion.identity);
                go.transform.SetParent(parentObject);
                go.gameObject.SetActive(true);*/
                var obj = ObjectManager.Instance.SpawnObject(place, StringKeys.MAP_PLACE, true);
                obj.transform.position = new Vector3(i * 1.5f, 0, j * 1.5f);
                obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
                obj.gameObject.name = $"Place_{i}_{j}";
                Vector2 spawnPoint = new Vector2(i * 1.5f, j * 1.5f);
                _square[i * 8 + j] = spawnPoint;
                _mapSquares[i, j] = obj.GetComponent<MapSquare>();
                _mapSquareDic.Add(spawnPoint, _mapSquares[i, j]);
            }
        }
        PawnManager.Instance.SpawnPoints = _square;
        PawnManager.Instance.MapSquareDic = _mapSquareDic;
        PawnManager.Instance.SpawnPawn();
    }
}
