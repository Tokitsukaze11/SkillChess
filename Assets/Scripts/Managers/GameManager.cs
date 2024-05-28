using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public event Action OnTurnEnd;
    public Func<bool> IsPlayerTurn;
    public void TurnEnd()
    {
        OnTurnEnd!.Invoke();
    }
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}
