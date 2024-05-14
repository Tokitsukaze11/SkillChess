using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawnController : MonoBehaviour
{
    public GameObject playerPawnPrefab;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(playerPawnPrefab, "PlayerPawn");
    }
    public void SpawnPlayerPawn(Vector2[] spawnPoints)
    {
        for(int i = 0; i < 3; i++)
        {
            var obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab, "PlayerPawn", true);
            obj.transform.position = new Vector3(spawnPoints[i].x, 1, spawnPoints[i].y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"PlayerPawn_{i}";
        }
    }
    public void DespawnPlayerPawn()
    {
        
    }
}
