using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPawnController : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPoint;
    public GameObject[] enemyPawnPrefab;
    public GameObject enemyKingPrefab;
    private List<Pawn> _enemyPawns = new List<Pawn>();
    [SerializeField] private PawnBehaviorUIController _pawnBehaviorUIController;
    [SerializeField] private AudioClip[] _pawnDieSounds;
    private void Awake()
    {
        PawnManager.Instance.OnResetPawns += ResetPawns;
        PawnManager.Instance.OnSpawnPawns += SpawnEnemyPawn;
    }
    private void ResetPawns()
    {
        if (_enemyPawns.Count <= 0)
            return;
        foreach(var pawn in _enemyPawns)
        {
            ObjectManager.Instance.RemoveObject(pawn.gameObject);
        }
        _enemyPawns.Clear();
    }
    private void SpawnEnemyPawn()
    {
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        Vector3 spawnPos = _spawnPoint.transform.position;
        
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
                
                int curRow = nowRow;
                int curCol = i * 2;
                int curIndex = curCol * row + curRow;
                var curKey = SquareCalculator.CurrentKey(curIndex);
            
                //obj.transform.position = new Vector3(curKey.x, 0, curKey.y);
                obj.GetComponent<NavMeshAgent>().enabled = false;
                obj.transform.position = Vector3.zero;
                obj.transform.position = spawnPos;
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
                pawn.OnPlayDieSound += PlayDieSound;
                var curMapSquare = SquareCalculator.CurrentMapSquare(curIndex);
                curMapSquare.CurPawn = pawn;
                pawn.CurMapSquare = curMapSquare;
                _enemyPawns.Add(pawn);
                pawn.OnDie += PawnDie;
                //obj.GetComponent<NavMeshAgent>().enabled = true;
                StartCoroutine(pawn.Co_MoveToDest(curMapSquare.transform.position));
            }
        }
        StartCoroutine(SetNavMeshAgentEnable());
    }
    private IEnumerator SetNavMeshAgentEnable()
    {
        yield return new WaitForSeconds(0.1f);
        _enemyPawns.ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);
        yield break;
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
    private void PlayDieSound()
    {
        var clip = _pawnDieSounds[Random.Range(0, _pawnDieSounds.Length)];
        SoundManager.Instance.PlaySfx(clip);
    }
}
