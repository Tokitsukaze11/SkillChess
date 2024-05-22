using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject _mainCanvas;
    public GameObject _uiDamageParticle;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_uiDamageParticle, StringKeys.DAMAGE);
    }
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
    public GameObject ShowUIParticle()
    {
        var particle = ObjectManager.Instance.SpawnObject(_uiDamageParticle, StringKeys.DAMAGE, true);
        particle.transform.SetParent(_mainCanvas.transform);
        return particle;
    }
}
