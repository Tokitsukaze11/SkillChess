using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnLockController
{
    private static HashSet<PopupObject> _popupObjects = new HashSet<PopupObject>();
    public static event Action<KeyCode,Action> OnKeyAction;
    public static void LockedPopup(PopupObject popupObject)
    {
        _popupObjects.Add(popupObject);
        OnKeyAction?.Invoke(KeyCode.Escape,UnLockPopup);
    }
    private static void TryUnlockPopup()
    {
        foreach (var popupObject in _popupObjects)
        {
            popupObject.UnlockPopup();
        }
        _popupObjects.Clear();
    }
    private static void UnLockPopup()
    {
        TryUnlockPopup();
    }
}
