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
    public Transform _spawnPoint;
    public GameObject[] playerPawnPrefab;
    public GameObject playerKingPrefab;
    [SerializeField] private PawnBehaviorUIController _pawnBehaviorUIController;
    private List<Pawn> _playerPawns = new List<Pawn>();
    public event Action PlayerTickHandler;
    public void SpawnPlayerPawn(int count = 0)
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject obj = null;
            if(i != 4)
                obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab[i], null, false);
            else
                obj = ObjectManager.Instance.SpawnObject(playerKingPrefab, null, false);
            //var obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab, null, false);
            
            int targetColumn = i*GlobalValues.ROW;
            int targetRow = 0;
            int targetIndex = targetRow + targetColumn;
            var curMapSquare = SquareCalculator.CurrentMapSquare(targetIndex);
            Vector2 curKey = SquareCalculator.CurrentKey(targetIndex);
            
            //obj.transform.position = new Vector3(curKey.x, 0, curKey.y);
            obj.transform.position = _spawnPoint.position;
            //obj.gameObject.GetComponent<NavMeshAgent>().destination = new Vector3(curKey.x, 0, curKey.y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"PlayerPawn_{i}";
            obj.SetActive(true);
            var pawn = obj.GetComponent<Pawn>();
            pawn._isPlayerPawn = true;
            pawn.OnPawnClicked += _pawnBehaviorUIController.PawnBehaviorUIPanelActive;
            pawn.OnCannotAction += _pawnBehaviorUIController.ButtonShake;
            curMapSquare.CurPawn = pawn;
            pawn.CurMapSquare = curMapSquare;
            pawn.OnDie += PawnDie;
            _playerPawns.Add(pawn);
            pawn._isCanClick = true; // TODO : If random player turn, change this
            StartCoroutine(pawn.Co_MoveToDest(curMapSquare.transform.position));
        }
        _pawnBehaviorUIController.UpdatePlayerPawns(_playerPawns);
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
        //ObjectManager.Instance.RemoveObject(diedPawn.gameObject, "PlayerPawn", true);
        _playerPawns.Remove(diedPawn);
        if(diedPawn.PawnType == PawnType.King)
        {
            Debug.Log("Game Over, Enemy Win");
            // TODO : If pawn is king, game over
        }
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject);
    }
}
