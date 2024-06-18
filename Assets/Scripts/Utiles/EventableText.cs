using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class EventableText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    private Camera _renderCamera;
    private Coroutine _mouseOverCoroutine;
    private string _linkId = null;
    private bool _isPanelLock = false;
    private void Awake()
    {
        _renderCamera = UIManager.Instance.renderCamera;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
        }
        _mouseOverCoroutine = StartCoroutine(CO_MouseOver());
    }
    private IEnumerator CO_MouseOver()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            if (_linkId == null)
            {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMeshProUGUI, Input.mousePosition, _renderCamera);
                if (linkIndex != -1)
                {
                    MouseChange(true);
                    var linkInfo = _textMeshProUGUI.textInfo.linkInfo[linkIndex];
                    var linkId = linkInfo.GetLinkID(); // Link ID
                    _linkId = linkId;
                    var linkText = _textMeshProUGUI.textInfo.linkInfo[linkIndex].GetLinkText();
                    //Debug.Log($"Link Index : {linkIndex}, Link ID : {linkId}, Link Text : {linkText}");
                    LinkManager.Instance.LinkEvent(linkId);
                }
            }
            /*if (time > 3)
            {
                yield break;
            }*/
            yield return null;
        }
        yield break;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseChange(false);
        if (_mouseOverCoroutine == null)
            return;
        StopCoroutine(_mouseOverCoroutine);
        if(_linkId != null)
            LinkManager.Instance.LinkEvent(_linkId, false);
    }
    private void MouseChange(bool isOn)
    {
        CursorController.SetPopupCursor(isOn);
    }
}
