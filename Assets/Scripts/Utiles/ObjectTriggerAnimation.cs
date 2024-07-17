using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerAnimation : MonoBehaviour
{
    public event Action OnAnimationTrigger;
    public event Action OnAnimationEndTrigger;
    public void TriggerEvent()
    {
        OnAnimationTrigger?.Invoke();
    }
    public void ResetTrigger()
    {
        OnAnimationTrigger = null;
    }
    public void EndTriggerEvent()
    {
        OnAnimationEndTrigger?.Invoke();
    }
    public void ResetEndTrigger()
    {
        OnAnimationEndTrigger = null;
    }
}
