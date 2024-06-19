using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    private Dictionary<KeyCode,Stack<Action>> _keyEvents = new Dictionary<KeyCode, Stack<Action>>();
    private Dictionary<KeyCode,Action> _keyActions = new Dictionary<KeyCode, Action>();
    private void Awake()
    {
        _keyActions.Add(KeyCode.Escape, EscapeKey);
        UpdateManager.Instance.OnUpdate += KeyChecker;
    }
    public void AttachKeyEvent(KeyCode keyCode, Action action)
    {
        if (!_keyEvents.TryGetValue(keyCode, out var keyEvent))
        {
            keyEvent = new Stack<Action>();
            _keyEvents.Add(keyCode, keyEvent);
        }
        if(keyEvent.Count < 3)
            keyEvent.Push(action);
    }
    public void DetachKeyEvent(KeyCode keyCode,Action action)
    {
        if (!_keyEvents.TryGetValue(keyCode, out var keyEvent))
            return;
        var detachEvent = keyEvent.ToList().Where(x => x == action).ToList();
        keyEvent = new Stack<Action>(keyEvent.Except(detachEvent));
    }
    private void KeyChecker()
    {
        foreach (var key in _keyActions.Keys.Where(Input.GetKeyDown))
        {
            _keyActions[key]?.Invoke();
        }
    }
    private void EscapeKey()
    {
        var escActions = _keyEvents[KeyCode.Escape];
        var action = escActions.Count > 1 ? escActions.Pop() : escActions.Peek();
        action?.Invoke();
    }
}
