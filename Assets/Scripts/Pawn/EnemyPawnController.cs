using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawnController : MonoBehaviour
{
    public GameObject enemyPawnPrefab;
    private List<Pawn> _enemyPawns = new List<Pawn>();
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
            int targetRow = i*8*2;
            obj.transform.position = new Vector3(spawnPoints[targetRow].x, 1, spawnPoints[7].y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"EnemyPawn_{i}";
            obj.GetComponent<MeshRenderer>().material.color = Color.magenta;
            var pawn = obj.GetComponent<SamplePawn>();
            pawn._isPlayerPawn = false;
            pawn._sortingGroup.sortingOrder = i; //TODO : 열을 기준으로 정렬
            var curMapSquare = PawnManager.Instance.GetCurrentMapSquare(new Vector2(spawnPoints[targetRow].x, spawnPoints[7].y));
            curMapSquare.CurPawn = pawn;
            pawn.CurMapSquare = curMapSquare;
            _enemyPawns.Add(pawn);
            pawn.OnDie += PawnDie;
        }
    }
    public void DespawnEnemyPawn()
    {
        
    }
    public void TurnChange(bool isEnemyTurn)
    {
        _enemyPawns.ForEach(x => x._isCanClick = isEnemyTurn);
    }
    private void PawnDie(Pawn diedPawn)
    {
        _enemyPawns.Remove(diedPawn);
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject, "EnemyPawn",true);
        if(diedPawn.PawnType == PawnType.King)
        {
            Debug.Log("Game Over");
            // TODO : If pawn is king, game over
        }
    }
}
