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
        _debugButton.onClick.AddListener(DebugFunc);
    }
    private void DebugFunc()
    {
        FindObjectOfType<MapSpawner>().ResetMapSquares();
    }
}
