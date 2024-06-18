using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnLockController
{
    private static HashSet<PopupObject> _popupObjects = new HashSet<PopupObject>();
    public static void Init()
    {
        UpdateManager.Instance.OnUpdate += OnUpdate;
    }
    public static void LockedPopup(PopupObject popupObject)
    {
        _popupObjects.Add(popupObject);
    }
    private static void TryUnlockPopup()
    {
        foreach (var popupObject in _popupObjects)
        {
            popupObject.UnlockPopup();
        }
        _popupObjects.Clear();
    }
    private static void OnUpdate()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            TryUnlockPopup();
        }
    }
}
