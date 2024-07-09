using System;
using System.Collections;
using System.Collections.Generic;
using OutlineFx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugUtil : MonoBehaviour
{
    
    public UniversalRendererData _universalRendererData;
    public OutlineFxFeature _outlineFxFeature = null;
    
    private void Start()
    {
        #if !UNITY_EDITOR
        Destroy(this);
        #endif
        
        _universalRendererData.rendererFeatures.ForEach(x =>
        {
            x.name = x.GetType().Name;
            if(x.name == "OutlineFxFeature")
            {
                _outlineFxFeature = x as OutlineFxFeature;
            }
        });
    }
}
