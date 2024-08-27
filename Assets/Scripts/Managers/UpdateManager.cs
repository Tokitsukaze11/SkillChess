using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : Singleton<UpdateManager>
{
    public event Action OnUpdate;
    /*private void Update()
    {
        OnUpdate?.Invoke();
    }*/
}
