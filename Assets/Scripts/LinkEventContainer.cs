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
        LinkManager.Instance.AttachLinkEvent("ID_RANGE_PREFERENCE_LESS",ID_RANGE_PREFERENCE_LESS);
        LinkManager.Instance.AttachLinkEvent("ID_RANGE_PREFERENCE", ID_RANGE_PREFERENCE);
        LinkManager.Instance.AttachLinkEvent("ID_HOWITZER", ID_HOWITZER);
        LinkManager.Instance.AttachLinkEvent("ID_TICK", ID_TICK);
        LinkManager.Instance.AttachLinkEvent("ID_DIAGONAL", ID_DIAGONAL);
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
            string description = "기물의 위치를 중심으로 한 상하좌우 4개의 방향을 의미합니다.\n거리는 상하좌우 방향으로만 계산되며, 대각선 방향은 직접적으로 고려되지 않습니다.";
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
    private void ID_RANGE_PREFERENCE_LESS(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "지정된 범위 이하의 거리에 있는 칸을 선택할 수 있습니다.\n거리는 상하좌우 방향으로만 계산되며, 대각선 방향은 직접적으로 고려되지 않습니다.";
            CreateAndInitPopup(description, position);
        }
        else
            RemovePopup();
    }
    private void ID_RANGE_PREFERENCE(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "지정된 범위와 정확히 일치하는 거리에 있는 칸만 선택할 수 있습니다.\n거리는 상하좌우 방향으로만 계산되며, 대각선 방향은 직접적으로 고려되지 않습니다.";
            CreateAndInitPopup(description, position);
        }
        else
            RemovePopup();
    }
    private void ID_HOWITZER(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "지정된 범위 이하의 거리에 있는 칸 중 타겟이 있는 칸만 선택할 수 있습니다.\n장애물의 존재를 무시하며, 거리는 상하좌우 방향으로만 계산됩니다.\n대각선 방향은 직접적으로 고려되지 않습니다.";
            CreateAndInitPopup(description, position);
        }
        else
            RemovePopup();
    }
    private void ID_TICK(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "각 플레이어의 턴이 돌아오는 주기를 의미하며, 주로 시전자의 다음 턴 시작을 기준으로 합니다.\n효과의 지속 시간 등이 이 단위로 측정됩니다.";
            CreateAndInitPopup(description, position);
        }
        else
            RemovePopup();
    }
    private void ID_DIAGONAL(bool isOn, Vector3 position)
    {
        if (isOn)
        {
            string description = "선택 지점을 중심으로 일정한 거리의 지점을 의미합니다.\n상하좌우의 직선 거리는 설정된 이하의 거리의 칸이 영향 범위입니다.\n대각선의 거리는 설정된 거리의 절반으로 영향을 받습니다.\n단, 소숫점 이하는 버림니다.";
            CreateAndInitPopup(description, position);
        }
        else
            RemovePopup();
    }
    #endregion
}
