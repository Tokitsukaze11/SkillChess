using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnManager : Singleton<PawnManager>
{
    private Vector2[] spawnPoints;
    public Vector2[] SpawnPoints
    {
        set
        {
            spawnPoints = value;
        }
    }
    [SerializeField] private PlayerPawnController _playerPawnController;
    [SerializeField] private EnemyPawnController _enemyPawnController;
    public void SpawnPawn()
    {
        _playerPawnController.SpawnPlayerPawn(spawnPoints);
        _enemyPawnController.SpawnEnemyPawn(spawnPoints);
    }
    public void DespawnPawn()
    {
        
    }
}
