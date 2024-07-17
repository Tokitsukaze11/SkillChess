using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    public void AsyncStartViaCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
