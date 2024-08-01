using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawnController : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    public GameObject[] enemyPawnPrefab;
    public GameObject enemyKingPrefab;
    private List<Pawn> _enemyPawns = new List<Pawn>();
    [SerializeField] private PawnBehaviorUIController _pawnBehaviorUIController;
    public void SpawnEnemyPawn(int count = 0)
    {
        for(int i = 0; i < 3; i++)
        {
            GameObject obj = null;
            if (i != 1)
                obj = ObjectManager.Instance.SpawnObject(enemyPawnPrefab[Random.Range(0, enemyPawnPrefab.Length)], null, false);
            else
                obj = ObjectManager.Instance.SpawnObject(enemyKingPrefab, null, false);
            // 적은 플레이어와 반대편에 배치
            int targetCol = i*GlobalValues.ROW*2;
            int targetRow = GlobalValues.ROW - 1;
            int curIndex = targetCol + targetRow;
            var curKey = SquareCalculator.CurrentKey(curIndex);
            
            //obj.transform.position = new Vector3(curKey.x, 0, curKey.y);
            obj.transform.position = _spawnPoint.position;
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"EnemyPawn_{i}";
            //obj.transform.rotation = Quaternion.Euler(0, 180, 0);
            //obj.GetComponent<MeshRenderer>().material.color = Color.magenta;
            obj.SetActive(true);
            var pawn = obj.GetComponent<Pawn>();
            pawn._isPlayerPawn = false;
            pawn._sortingGroup.sortingOrder = i; //TODO : 열을 기준으로 정렬
            pawn.OnPawnClicked += _pawnBehaviorUIController.PawnBehaviorUIPanelActive;
            pawn.OnCannotAction += _pawnBehaviorUIController.ButtonShake;
            var curMapSquare = SquareCalculator.CurrentMapSquare(curIndex);
            curMapSquare.CurPawn = pawn;
            pawn.CurMapSquare = curMapSquare;
            _enemyPawns.Add(pawn);
            pawn.OnDie += PawnDie;
            StartCoroutine(pawn.Co_MoveToDest(curMapSquare.transform.position));
        }
    }
    public void DespawnEnemyPawn()
    {
        
    }
    public List<Pawn> GetPawns()
    {
        return _enemyPawns;
    }
    public void TurnChange(bool isEnemyTurn)
    {
        _enemyPawns.ForEach(x => x._isCanClick = isEnemyTurn);
        _enemyPawns.ForEach(x => x._isPlayerPawn = isEnemyTurn);
        if(isEnemyTurn)
            _pawnBehaviorUIController.UpdatePlayerPawns(_enemyPawns);
    }
    private void PawnDie(Pawn diedPawn)
    {
        _enemyPawns.Remove(diedPawn);
        if(diedPawn.PawnType == PawnType.King)
        {
            Debug.Log("Game Over, Player Win");
            GameManager.Instance.GameEnd(true);
            // TODO : If pawn is king, game over
        }
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject);
    }
}
