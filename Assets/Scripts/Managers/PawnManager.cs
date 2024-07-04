using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PawnManager : Singleton<PawnManager>
{
    public GameObject _damageTextParticle;
    private Dictionary<Vector2, MapSquare> _mapSquareDic;
    [SerializeField] private PlayerPawnController _playerPawnController;
    [SerializeField] private EnemyPawnController _enemyPawnController;
    public event Action OnPlayerTurn;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_damageTextParticle, StringKeys.DAMAGE);
    }
    public void SetMapSquareDic(Dictionary<Vector2, MapSquare> mapSquareDic)
    {
        _mapSquareDic = mapSquareDic;
        SquareCalculator.MapSquareDic = _mapSquareDic;
    }
    public void SpawnPawn() // TODO : Will maybe get count of pawn
    {
        _playerPawnController.SpawnPlayerPawn();
        _enemyPawnController.SpawnEnemyPawn();
    }
    public void DespawnPawn()
    {
        
    }
    public void TurnChange(bool isPlayerTurn)
    {
        if (isPlayerTurn) OnPlayerTurn?.Invoke();
        _playerPawnController.TurnChange(isPlayerTurn);
        //_enemyPawnController.TurnChange(!isPlayerTurn);
    }
    public void ResetSquaresColor()
    {
        _mapSquareDic.Values.ToList().ForEach(x => x.ResetColor());
    }
}
