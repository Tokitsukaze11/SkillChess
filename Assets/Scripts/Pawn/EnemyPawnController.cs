using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawnController : MonoBehaviour
{
    public GameObject enemyPawnPrefab;
    private List<Pawn> _enemyPawns;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(enemyPawnPrefab, "EnemyPawn");
    }
    public void SpawnEnemyPawn(Vector2[] spawnPoints)
    {
        for(int i = 0; i < 3; i++)
        {
            var obj = ObjectManager.Instance.SpawnObject(enemyPawnPrefab, "EnemyPawn", true);
            // 적은 플레이어와 반대편에 배치
            obj.transform.position = new Vector3(spawnPoints[i*8].x, 1, spawnPoints[7].y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"EnemyPawn_{i}";
            obj.GetComponent<MeshRenderer>().material.color = Color.magenta;
            var pawn = obj.GetComponent<SamplePawn>();
            pawn._isPlayerPawn = false;
            var curMapSquare = PawnManager.Instance.GetCurrentMapSquare(new Vector2(spawnPoints[i*8].x, spawnPoints[7].y));
            curMapSquare.CurPawn = pawn;
            pawn.CurMapSquare = curMapSquare;
            _enemyPawns.Add(pawn);
        }
    }
    public void DespawnEnemyPawn()
    {
        
    }
}
