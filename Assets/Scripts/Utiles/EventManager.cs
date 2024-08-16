using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public event Action<int, int> OnGameStart;
    public event Action OnTitle;
    
    public void GameStart(int row, int col)
    {
        OnGameStart?.Invoke(row, col);
    }
    public void GoTitle()
    {
        OnTitle?.Invoke();
    }
}
