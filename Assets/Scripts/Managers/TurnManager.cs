using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool _isPlayer1Turn = true;
    private void Awake()
    {
        GameManager.Instance.OnTurnEnd += TurnChange;
        GameManager.Instance.IsPlayer1Turn = () => _isPlayer1Turn;
        GameManager.Instance.OnGameRestart += StartGame;
    }
    private void StartGame()
    {
        _isPlayer1Turn = true;
        // TODO : First Turn Event
    }
    private void TurnChange()
    {
        _isPlayer1Turn = !_isPlayer1Turn;
        Action action = _isPlayer1Turn ? PlayerTurn : EnemyTurn;
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
