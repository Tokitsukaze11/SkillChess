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
        LinkManager.Instance.AttachLinkEvent("ID_OBSTACLES", ID_OBSTACLES);
        LinkManager.Instance.AttachLinkEvent("ID_IGNORE_OBSTACLES", ID_IGNORE_OBSTACLES);
    }
    private void CreateAndInitPopup(string description, Vector3 position)
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
        popup.GetComponent<LinkEventPopup>().Init(description, position);
    }
    private void RemovePopup()
    {
        _popupObjects.TryGetValue(StringKeys.LINK_EVENT_POPUP_OBJECT, out var popup);
        if (popup == null)
            return;
        ObjectManager.Instance.RemoveObject(popup, StringKeys.LINK_EVENT_POPUP_OBJECT);
        _popupObjects.Remove(StringKeys.LINK_EVENT_POPUP_OBJECT);
    }
    #region Link Event
    private void ID_STRAIGHT(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "기물의 위치를 중심으로 한 상하좌우 4개의 방향을 의미합니다.\n대각선은 고려하지 않습니다.";
            CreateAndInitPopup(description, position);
        }
        else
        {
            RemovePopup();
        }
    }
    private void ID_OBSTACLES(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "장애물의 존재를 고려합니다.\n행동 반경 안에 장애물이 있을 시, 장애물 너머의 행동 반경은 행동할 수 없습니다.";
            CreateAndInitPopup(description, position);
        }
        else
        {
            RemovePopup();
        }
    }
    private void ID_IGNORE_OBSTACLES(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "장애물의 존재를 무시합니다.\n행동 반경 안에 장애물이 있어도 행동할 수 있습니다.";
            CreateAndInitPopup(description, position);
        }
        else
        {
            RemovePopup();
        }
    }
    #endregion
}
