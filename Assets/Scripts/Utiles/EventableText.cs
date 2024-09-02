using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.InternalUtil;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class EventableText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    private Camera _renderCamera;
    private string _linkId = null;
    private IDisposable _mouseOverDisposable;
    private void Awake()
    {
        _renderCamera = UIManager.Instance.renderCamera;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_mouseOverDisposable == null)
            _mouseOverDisposable = Observable.FromMicroCoroutine(Co_MouseOver).Subscribe();
        else
        {
            _mouseOverDisposable.Dispose();
            _mouseOverDisposable = Observable.FromMicroCoroutine(Co_MouseOver).Subscribe();
        }
    }
    private IEnumerator Co_MouseOver()
    {
        while (true)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMeshProUGUI, Input.mousePosition, _renderCamera);
            if (linkIndex != -1)
            {
                MouseChange(true);
                var linkInfo = _textMeshProUGUI.textInfo.linkInfo[linkIndex];
                var linkId = linkInfo.GetLinkID(); // Link ID
                //var linkText = _textMeshProUGUI.textInfo.linkInfo[linkIndex].GetLinkText();
                //Debug.Log($"Link Index : {linkIndex}, Link ID : {linkId}, Link Text : {linkText}");
                if (_linkId != linkId)
                {
                    _linkId = linkId;
                    //Vector3 mousePos = _renderCamera.ScreenToWorldPoint(Input.mousePosition);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance._masterPanel, Input.mousePosition, _renderCamera, out var mousePos);
                    mousePos.y += _textMeshProUGUI.fontSize * 2;
                    LinkManager.Instance.LinkEvent(linkId,true, mousePos);
                }
            }
            else
            {
                MouseChange(false);
                if (_linkId != null)
                {
                    LinkManager.Instance.LinkEvent(_linkId, false, default);
                    _linkId = null;
                }
            }
            yield return null;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseChange(false);
        _mouseOverDisposable?.Dispose();
        if(_linkId != null)
            LinkManager.Instance.LinkEvent(_linkId, false);
    }
    private void MouseChange(bool isOn)
    {
        CursorController.SetPopupCursor(isOn);
    }
}
