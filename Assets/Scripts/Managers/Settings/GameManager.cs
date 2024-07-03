using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Idle,
    Play,
    Pause,
}
public class GameManager : Singleton<GameManager>
{
    private GameState _eGameState = GameState.Idle;
    public GameState GameState => _eGameState;
    public Camera mainCamera;
    public event Action OnTurnEnd;
    public Func<bool> IsPlayerTurn;
    private int _targetFPS = 120;
    public int TargetFPS => _targetFPS;
    public event Action<GameState> OnGameStateChanged;
    public Texture2D[] _cursorTextures;
    public void TurnEnd()
    {
        OnTurnEnd!.Invoke();
    }
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Application.targetFrameRate = _targetFPS;
        _eGameState = GameState.Play; // TODO : Change to GameState.Idle
        CursorController.InitCursor(_cursorTextures);
    }
    public void GameStateChange(GameState gameState)
    {
        _eGameState = gameState;
        OnGameStateChanged?.Invoke(gameState);
    }
}
