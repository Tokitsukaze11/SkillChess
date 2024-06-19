using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDescriptController : MonoBehaviour
{
    public PopupObject _movePopup;
    public PopupObject _attackPopup;
    public PopupObject _defendPopup;
    public PopupObject _skillPopup;
    
    public TextMeshProUGUI _descriptionText;
    public Image _descriptionPopupImage;
    private Coroutine _panelAnimCoroutine = null;
    private RectTransform _panelRectTransform;
    
    public EventableText _eventableText;
    
    private void Awake()
    {
        _movePopup.OnMouseOverPopup += PopupControl;
        _attackPopup.OnMouseOverPopup += PopupControl;
        _defendPopup.OnMouseOverPopup += PopupControl;
        _skillPopup.OnMouseOverPopup += PopupControl;
        
        _movePopup.OnMouseOverNew += UnlockOtherPopUp;
        _attackPopup.OnMouseOverNew += UnlockOtherPopUp;
        _defendPopup.OnMouseOverNew += UnlockOtherPopUp;
        _skillPopup.OnMouseOverNew += UnlockOtherPopUp;
        
        _panelRectTransform = _descriptionPopupImage.rectTransform;
    }
    private void Start()
    {
        _descriptionPopupImage.gameObject.SetActive(false);
        _descriptionPopupImage.rectTransform.sizeDelta = new Vector2(800, 0);
        PopupControl(false);
    }
    private void PopupControl(bool isOn,string description = null)
    {
        _descriptionText.text = isOn ? description : "";

        Action UIAction = () =>
        {
            Coroutine panelAnimCoroutine = null;
            if (isOn)
            {
                if (_panelAnimCoroutine != null)
                {
                    StopCoroutine(_panelAnimCoroutine);
                }
                panelAnimCoroutine = StartCoroutine(Co_PanelAnim(true));
            }
            else
            {
                if (_panelAnimCoroutine != null)
                {
                    StopCoroutine(_panelAnimCoroutine);
                }
                panelAnimCoroutine = StartCoroutine(Co_PanelAnim(false));
            }
            _panelAnimCoroutine = panelAnimCoroutine;
        };
        UIManager.Instance.UpdateUI(UIAction);
    }
    private IEnumerator Co_PanelAnim(bool isOn)
    {
        Vector2 targetSize = isOn ? new Vector2(800, 230) : new Vector2(800, 0);
        if(isOn)
            _descriptionPopupImage.gameObject.SetActive(true);

        float time = 0;
        int frame = GameManager.Instance.TargetFPS;
        float duration = isOn ? 4f * (frame/60f) : 1f * (frame/60f);

        while (true)
        {
            time += Time.deltaTime;
            _panelRectTransform.sizeDelta = Vector2.Lerp(_panelRectTransform.sizeDelta, targetSize, time/duration);
            if (Vector2.Distance(_panelRectTransform.sizeDelta, targetSize) < 0.1f)
            {
                _panelRectTransform.sizeDelta = targetSize;
                if (!isOn) _descriptionPopupImage.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }
    private void UnlockOtherPopUp(PopupObject popupObject)
    {
        List<PopupObject> popupObjects = new List<PopupObject>
        {
            _movePopup,
            _attackPopup,
            _defendPopup,
            _skillPopup
        };
        popupObjects.Where(popup => popup != popupObject).ToList().ForEach(popup => popup.UnlockAsForce());
    }
}
