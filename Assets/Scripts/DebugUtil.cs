using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtil : MonoBehaviour
{
    private void Start()
    {
        #if !UNITY_EDITOR
        Destroy(this);
        #endif
    }
}
