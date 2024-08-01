using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Idle,
    Play,
    Pause,
    End
}
public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public Texture2D[] _cursorTextures;
    private int _targetFPS = 120;
    private bool _isEndGame = false;
    private GameState _eGameState = GameState.Idle;
    public GameState GameState => _eGameState;
    public int TargetFPS => _targetFPS;
    public event Action OnTurnEnd;
    public Func<bool> IsPlayerTurn;
    public event Action<GameState> OnGameStateChanged;
    public event Action<bool> OnGameEnd;
    public void TurnEnd()
    {
        if(!_isEndGame)
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
    public void GameEnd(bool isPlayerWin = false)
    {
        _isEndGame = true;
        GameStateChange(GameState.End);
        OnGameEnd?.Invoke(isPlayerWin);
    }
}
