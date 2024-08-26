using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OutlineFx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DebugUtil : MonoBehaviour
{
    public Button _debugButton;
    
    private void Start()
    {
        #if !UNITY_EDITOR
        Destroy(this);
        #endif
        //_debugButton.onClick.AddListener(DebugFunc);
    }
    private void DebugFunc()
    {
        /*int randomRow = Random.Range(8, 20);
        int randomCol = Random.Range(8, 20);
        GlobalValues.ROW = randomRow;
        GlobalValues.COL = randomCol;
        FindObjectOfType<MapSpawner>().ResetMapSquares();*/
    }
}
