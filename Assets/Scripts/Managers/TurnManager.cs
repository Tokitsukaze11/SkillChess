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
        GameManager.Instance.IsPlayerTurn = () => _isPlayerTurn;
    }
    private void TurnChange()
    {
        _isPlayerTurn = !_isPlayerTurn;
        Action action = _isPlayerTurn ? PlayerTurn : EnemyTurn;
        action!.Invoke();
    }
    private void PlayerTurn()
    {
        PawnManager.Instance.TurnChange(true);
        Debug.Log("<color=green>Player Turn</color>");
    }
    private void EnemyTurn()
    {
        PawnManager.Instance.TurnChange(false);
        Debug.Log("<color=red>Enemy Turn</color>");
    }
}
