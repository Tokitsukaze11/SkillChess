using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public event Action OnTurnEnd;
    public Func<bool> IsPlayerTurn;
    private int _targetFPS = 60;
    public int TargetFPS => _targetFPS;
    public void TurnEnd()
    {
        OnTurnEnd!.Invoke();
    }
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Application.targetFrameRate = _targetFPS;
    }
}
