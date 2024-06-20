using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkManager : Singleton<LinkManager>
{
    private Dictionary<string,Action<bool,Vector3>> linkEvents = new Dictionary<string, Action<bool,Vector3>>();
    public void AttachLinkEvent(string linkCode, Action<bool,Vector3> action)
    {
        linkEvents.TryAdd(linkCode, action);
    }
    public void LinkEvent(string linkCode, bool isOn = true, Vector3 position = default)
    {
        if (!linkEvents.TryGetValue(linkCode, out var linkEvent))
            return;
        linkEvent?.Invoke(isOn,position);
    }
}
