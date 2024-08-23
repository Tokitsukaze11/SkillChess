using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerPawnController : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPoint;
    public GameObject[] playerPawnPrefab;
    public GameObject playerKingPrefab;
    private List<Pawn> _playerPawns = new List<Pawn>();
    [SerializeField] private PawnBehaviorUIController _pawnBehaviorUIController;
    [SerializeField] private AudioClip[] _pawnDieSounds;
    public event Action PlayerTickHandler;
    private void Awake()
    {
        PawnManager.Instance.OnResetPawns += ResetPawns;
        PawnManager.Instance.OnSpawnPawns += SpawnPlayerPawn;
    }
    private void ResetPawns()
    {
        if (_playerPawns.Count <= 0)
            return;
        foreach(var pawn in _playerPawns)
        {
            ObjectManager.Instance.RemoveObject(pawn.gameObject);
        }
        _playerPawns.Clear();
    }
    private void SpawnPlayerPawn()
    {
        int row = GlobalValues.ROW;
        int col = GlobalValues.COL;
        
        Vector3 spawnPos = _spawnPoint.transform.position;
        
        // 0과 1행에 각각 홀수 열에 배치(i가 짝수일 때). 킹은 0행의 중앙 즈음에 배치.(대충 col/2에 위치 할 듯)

        for (int nowRow = 0; nowRow < 2; nowRow++)
        {
            for (int i = 0; i < col/2; i++)
            {
                GameObject obj = null;
                
                if(i == col/2/2 && nowRow == 0)
                    obj = ObjectManager.Instance.SpawnObject(playerKingPrefab, null, false);
                else
                    obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab[Random.Range(0,playerPawnPrefab.Length)], null, false);
                
                int curRow = nowRow;
                int curCol = i * 2;
                int targetIndex = curCol * row + curRow;
                var curMapSquare = SquareCalculator.CurrentMapSquare(targetIndex);
                Vector2 curKey = SquareCalculator.CurrentKey(targetIndex);
            
                obj.GetComponent<NavMeshAgent>().enabled = false;
                obj.transform.position = Vector3.zero;
                obj.transform.position = spawnPos;
                obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
                obj.gameObject.name = $"PlayerPawn_{i}";
                obj.SetActive(true);
                var pawn = obj.GetComponent<Pawn>();
                pawn._isPlayerPawn = true;
                pawn.OnPawnClicked += _pawnBehaviorUIController.PawnBehaviorUIPanelActive;
                pawn.OnCannotAction += _pawnBehaviorUIController.ButtonShake;
                pawn.OnPlayDieSound += PlayDieSound;
                curMapSquare.CurPawn = pawn;
                pawn.CurMapSquare = curMapSquare;
                pawn.OnDie += PawnDie;
                _playerPawns.Add(pawn);
                pawn._isCanClick = true; // TODO : If random player turn, change this
                StartCoroutine(pawn.Co_MoveToDest(curMapSquare.transform.position));
            }
        }
        StartCoroutine(SetNavMeshAgentEnable());
        _pawnBehaviorUIController.UpdatePlayerPawns(_playerPawns);
    }
    private IEnumerator SetNavMeshAgentEnable()
    {
        yield return new WaitForSeconds(0.1f);
        _playerPawns.ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);
        yield break;
    }
    public void DespawnPlayerPawn()
    {
        _playerPawns.ForEach(x => ObjectManager.Instance.RemoveObject(x.gameObject, "PlayerPawn", true));
        _playerPawns.Clear();
    }
    public List<Pawn> GetPawns()
    {
        return _playerPawns;
    }
    public void TurnChange(bool isPlayerTurn)
    {
        _playerPawns.ForEach(x => x._isCanClick = isPlayerTurn);
        if(isPlayerTurn)
            PlayerTickHandler?.Invoke();
        _playerPawns.ForEach(x => x._isPlayerPawn = isPlayerTurn);
        if(isPlayerTurn)
            _pawnBehaviorUIController.UpdatePlayerPawns(_playerPawns);
    }
    private void PawnDie(Pawn diedPawn)
    {
        _playerPawns.Remove(diedPawn);
        if(diedPawn.PawnType == PawnType.King)
        {
            GameManager.Instance.GameEnd();
        }
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject);
    }
    private void PlayDieSound()
    {
        var clip = _pawnDieSounds[Random.Range(0, _pawnDieSounds.Length)];
        SoundManager.Instance.PlaySfx(clip);
    }
}
