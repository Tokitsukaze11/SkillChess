using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : Singleton<UpdateManager>
{
    public event Action OnUpdate;
    void Update()
    {
        OnUpdate?.Invoke();
    }
}
