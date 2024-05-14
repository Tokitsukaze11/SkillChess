using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool _isPlayerTurn = true;
    private void Awake()
    {
        GameManager.Instance.OnTurnEnd += TurnChange;
    }
    private void TurnChange()
    {
        _isPlayerTurn = !_isPlayerTurn;
        Action action = _isPlayerTurn ? PlayerTurn : EnemyTurn;
        action!.Invoke();
    }
    private void PlayerTurn()
    {
        Debug.Log("Player Turn");
    }
    private void EnemyTurn()
    {
        Debug.Log("Enemy Turn");
    }
}
