using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkEventContainer : MonoBehaviour
{
    public GameObject _popupObject;
    private Dictionary<string,GameObject> _popupObjects = new Dictionary<string, GameObject>();
    private void Awake()
    {
        ObjectManager.Instance.MakePool(_popupObject, StringKeys.LINK_EVENT_POPUP_OBJECT);
        LinkManager.Instance.AttachLinkEvent("ID_STRAIGHT", ID_STRAIGHT);
    }
    #region Link Event
    private void ID_STRAIGHT(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            _popupObjects.TryGetValue(StringKeys.LINK_EVENT_POPUP_OBJECT, out var popup);
            if (popup != null)
            {
                popup.GetComponent<LinkEventPopup>().UpdatePosition(position);
                return;
            }
            popup = ObjectManager.Instance.SpawnObject(_popupObject, StringKeys.LINK_EVENT_POPUP_OBJECT);
            popup.transform.SetParent(UIManager.Instance._mainCanvas.transform, false);
            _popupObjects.Add(StringKeys.LINK_EVENT_POPUP_OBJECT, popup);
            string description = "직선 방향";
            popup.GetComponent<LinkEventPopup>().Init(description, position);
        }
        else
        {
            _popupObjects.TryGetValue(StringKeys.LINK_EVENT_POPUP_OBJECT, out var popup);
            if (popup == null)
                return;
            ObjectManager.Instance.RemoveObject(popup, StringKeys.LINK_EVENT_POPUP_OBJECT);
            _popupObjects.Remove(StringKeys.LINK_EVENT_POPUP_OBJECT);
        }
    }
    #endregion
}
