using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public event Action<int, int> OnGameStartHaveParam;
    public event Action OnGameStart;
    private void Awake()
    {
        GameManager.Instance.OnGameStart += GameStart;
        GameManager.Instance.OnGameRestart += GameRestart;
    }
    private void GameStart()
    {
        GameStart(GlobalValues.ROW, GlobalValues.COL);
        DelayGameStart();
    }
    private void GameRestart()
    {
        DelayGameStart();
    }
    private void GameStart(int row, int col)
    {
        OnGameStartHaveParam?.Invoke(row, col);
    }
    private void DelayGameStart()
    {
        Observable.Timer(System.TimeSpan.FromSeconds(5)).Subscribe(_ => OnGameStart?.Invoke());
    }
}
