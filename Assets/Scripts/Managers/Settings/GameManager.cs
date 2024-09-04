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
    public Func<bool> IsPlayer1Turn;
    private event Action<GameState> OnGameStateChanged;
    public event Action<bool> OnGameEnd;
    public event Action OnGameRestart;
    public event Action OnGameStart;
    public event Action OnTitle;
    public void GameStart()
    {
        OnGameStart?.Invoke();
    }
    public void TurnEnd()
    {
        if(!_isEndGame)
            OnTurnEnd!.Invoke();
    }
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Application.targetFrameRate = _targetFPS;
        CursorController.InitCursor(_cursorTextures);
    }
    public void CloseGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
        Application.Quit();
        #endif
    }
    public void AttachGameStateChanged(Action<GameState> action)
    {
        OnGameStateChanged += action;
    }
    public void DetachGameStateChanged(Action<GameState> action)
    {
        OnGameStateChanged -= action;
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
    public void GameRestart()
    {
        _isEndGame = false;
        GameStateChange(GameState.Play);
        OnGameRestart?.Invoke();
    }
    public void GameTitle()
    {
        OnTitle?.Invoke();
    }
}
