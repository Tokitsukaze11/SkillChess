using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawnController : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPoint;
    public GameObject[] enemyPawnPrefab;
    public GameObject enemyKingPrefab;
    private List<Pawn> _enemyPawns = new List<Pawn>();
    [SerializeField] private PawnBehaviorUIController _pawnBehaviorUIController;
    public void ResetPawns()
    {
        if (_enemyPawns.Count <= 0)
            return;
        foreach(var pawn in _enemyPawns)
        {
            ObjectManager.Instance.RemoveObject(pawn.gameObject);
        }
        _enemyPawns.Clear();
    }
    public void SpawnEnemyPawn(int count = 0)
    {
        /*for(int i = 0; i < 3; i++)
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
        }*/
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        // row-1(0부터 시작하기에 row-1이 마지막 행)과 row-2행에 각각 홀수 열에 배치(i가 짝수일 때). 킹은 row행의 중앙 즈음에 배치.(대충 col/2에 위치 할 듯)

        for (int nowRow = row-1; nowRow > row-3; nowRow--)
        {
            for (int i = 0; i < col/2; i++)
            {
                GameObject obj = null;
                
                if(i == col/2/2 && nowRow == row-1)
                    obj = ObjectManager.Instance.SpawnObject(enemyKingPrefab, null, false);
                else
                    obj = ObjectManager.Instance.SpawnObject(enemyPawnPrefab[Random.Range(0,enemyPawnPrefab.Length)], null, false);

                int targetCol = i * 2 * GlobalValues.ROW;
                int targetRow = nowRow;
                int curIndex = targetCol + targetRow;
                var curKey = SquareCalculator.CurrentKey(curIndex);
            
                //obj.transform.position = new Vector3(curKey.x, 0, curKey.y);
                obj.transform.position = _spawnPoint.transform.position;
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
            GameManager.Instance.GameEnd(true);
        }
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject);
    }
}
