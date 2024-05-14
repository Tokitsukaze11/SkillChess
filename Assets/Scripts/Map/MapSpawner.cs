using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("Debugging Spawn")]
    public GameObject place;
    
    private Vector2[] _places = new Vector2[64]; // x: 행, z: 열
    
    private void Awake()
    {
        ObjectManager.Instance.MakePool(place, StringKeys.MAP_PLACE);
    }
    private void Start()
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
                _places[i * 8 + j] = new Vector2(i * 1.5f, j * 1.5f);
            }
        }
        PawnManager.Instance.SpawnPoints = _places;
        PawnManager.Instance.SpawnPawn();
    }
}
