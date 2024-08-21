using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PawnManager : Singleton<PawnManager>
{
    public GameObject _damageTextParticle;
    public GameObject _healTextParticle;
    private Dictionary<Vector2, MapSquare> _mapSquareDic;
    [SerializeField] private PlayerPawnController _playerPawnController;
    [SerializeField] private EnemyPawnController _enemyPawnController;
    public event Action OnPlayerTurn;
    public event Action OnEnemyTurn;
    public event Action OnResetPawns;
    public event Action OnSpawnPawns;
    public event Action OnSpawnComplete;
    public event Action OnTurnChange;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_damageTextParticle, StringKeys.DAMAGE);
        ObjectManager.Instance.MakePool(_healTextParticle, StringKeys.HEAL);
    }
    public void SetMapSquareDic(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        _mapSquareDic = mapSquareDic;
        SquareCalculator.MapSquareDic = _mapSquareDic;
    }
    public void ResetPawns()
    {
        /*_playerPawnController.ResetPawns();
        _enemyPawnController.ResetPawns();*/
        OnResetPawns?.Invoke();
    }
    public void SpawnPawn()
    {
        /*_playerPawnController.SpawnPlayerPawn();
        _enemyPawnController.SpawnEnemyPawn();*/
        OnSpawnPawns?.Invoke();
        OnSpawnComplete?.Invoke();
    }
    public void DespawnPawn()
    {
        
    }
    public void TurnChange(bool isPlayerTurn)
    {
        if (isPlayerTurn) OnPlayerTurn?.Invoke();
        else OnEnemyTurn?.Invoke();
        _playerPawnController.TurnChange(isPlayerTurn);
        _enemyPawnController.TurnChange(!isPlayerTurn);
        OnTurnChange?.Invoke();
    }
    public void ResetSquaresColor()
    {
        _mapSquareDic.Values.ToList().ForEach(x => x.ResetColor());
    }
    public List<Pawn> GetPawns(bool isPlayerPawn)
    {
        return isPlayerPawn ? _playerPawnController.GetPawns() : _enemyPawnController.GetPawns();
    } 
}
