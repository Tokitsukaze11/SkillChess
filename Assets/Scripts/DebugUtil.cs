using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OutlineFx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DebugUtil : MonoBehaviour
{
    public Button _debugButton;
    
    private void Start()
    {
        #if !UNITY_EDITOR
        Destroy(this);
        #endif
        _debugButton.onClick.AddListener(TryKillPawn);
    }
    private void TryKillPawn()
    {
        List<MapSquare> allSquares = new List<MapSquare>();
        for(int i = 0; i < GlobalValues.ROW * GlobalValues.COL; i++)
        {
            allSquares.Add(SquareCalculator.CurrentMapSquare(i));
        }
        allSquares.Where(x => x.CurPawn != null).ToList().Where(x => x.CurPawn.PawnType == PawnType.King && !x.CurPawn._isPlayerPawn).ToList().ForEach(x => x.CurPawn.TakeDamage(100));
    }
}
