using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkManager : Singleton<LinkManager>
{
    private Dictionary<string,Action<bool>> linkEvents = new Dictionary<string, Action<bool>>();
    private void Awake()
    {
        AttachLinkEvent("ID_STRAIGHT", (isOn) =>
        {
            Debug.Log($"ID_STRAIGHT : {isOn}");
        }); // Test
    }
    public void AttachLinkEvent(string linkCode, Action<bool> action)
    {
        linkEvents.TryAdd(linkCode, action);
    }
    public void LinkEvent(string linkCode, bool isOn = true)
    {
        if (!linkEvents.TryGetValue(linkCode, out var linkEvent))
            return;
        linkEvent?.Invoke(isOn);
    }
}
