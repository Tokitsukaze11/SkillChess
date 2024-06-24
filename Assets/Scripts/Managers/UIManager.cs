using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Camera renderCamera;
    public GameObject _mainCanvas;
    public RectTransform _masterPanel;
    public void UpdateUI([NotNull] Action uiAction)
    {
        if (uiAction == null)
            throw new System.Exception("uiAction is null!");
        uiAction.Invoke();
    }
    public void UpdateUI<T>([NotNull] Action<T> uiAction, T value)
    {
        if (uiAction == null)
            throw new System.Exception("uiAction is null!");
        uiAction.Invoke(value);
    }
}
