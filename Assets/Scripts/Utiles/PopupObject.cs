using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public event Action<bool,string> OnMouseOverPopup;
    private Coroutine _mouseOverCoroutine;
    private bool _isPanelLock = false;
    [SerializeField] private Image _timeSlider;
    public void InitDescription(DescriptObject descriptObject)
    {
        description = descriptObject.Description;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_isPanelLock)
            return;
        OnMouseOverPopup?.Invoke(true,description);
        _timeSlider.fillAmount = 0;
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
            _timeSlider.fillAmount = time / 3;
            if (time > 3)
            {
                _isPanelLock = true;
                UnLockController.LockedPopup(this);
                yield break;
            }
            yield return null;
        }
        yield break;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(!_isPanelLock)
        {
            OnMouseOverPopup?.Invoke(false, null);
            _timeSlider.fillAmount = 0;
        }
        if (_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
        }
    }
    public void UnlockPopup()
    {
        _isPanelLock = false;
        OnMouseOverPopup!.Invoke(false, null);
        _timeSlider.fillAmount = 0;
    }
}
