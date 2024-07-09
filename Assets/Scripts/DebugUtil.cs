using System;
using System.Collections;
using System.Collections.Generic;
using OutlineFx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugUtil : MonoBehaviour
{
    public OutlineFxFeature _outlineFxFeature;
    
    private void Start()
    {
        #if !UNITY_EDITOR
        Destroy(this);
        #endif
    }
}
