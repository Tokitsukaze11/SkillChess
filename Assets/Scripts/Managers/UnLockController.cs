using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnLockController
{
    private static PopupObject _popupObject;
    public static event Action<KeyCode,Action> OnKeyAction;
    public static void LockedPopup(PopupObject popupObject)
    {
        if(ReferenceEquals(_popupObject,null))
            OnKeyAction?.Invoke(KeyCode.Escape,UnLockPopup);
        _popupObject = popupObject;
    }
    private static void TryUnlockPopup()
    {
        if(ReferenceEquals(_popupObject,null))
            return;
        _popupObject.UnlockPopup();
        _popupObject = null;
    }
    private static void UnLockPopup()
    {
        TryUnlockPopup();
    }
}
