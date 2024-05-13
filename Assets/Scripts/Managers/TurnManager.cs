using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.OnTurnEnd += TurnChange;
    }
    private void TurnChange()
    {
        
    }
}
