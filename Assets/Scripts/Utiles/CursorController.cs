using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorController
{
    private static Texture2D _normalCursor;
    private static Texture2D _popupCursor;
    public static void InitCursor(Texture2D[] cursorTextures)
    {
        _normalCursor = cursorTextures[0];
        _popupCursor = cursorTextures[1];
    }
    public static void SetPopupCursor(bool isPopup)
    {
        Cursor.SetCursor(isPopup ? _popupCursor : _normalCursor, Vector2.zero, CursorMode.Auto);
    }
}
