using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action OnTurnEnd;
    public void TurnEnd()
    {
        OnTurnEnd!.Invoke();
    }
}
