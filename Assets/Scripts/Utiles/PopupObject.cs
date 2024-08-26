using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public event Action<bool,string> OnMouseOverPopup;
    public event Action<PopupObject> OnMouseOverNew;
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
        OnMouseOverNew?.Invoke(this);
        _timeSlider.fillAmount = 0;
        if (_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
        }
        _mouseOverCoroutine = StartCoroutine(Co_MouseOver());
    }
    private IEnumerator Co_MouseOver()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            _timeSlider.fillAmount = time / 1;
            if (time > 1)
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
    public void UnlockAsForce()
    {
        _isPanelLock = false;
        _timeSlider.fillAmount = 0;
    }
    public void StopFillAnim()
    {
        if (_mouseOverCoroutine != null)
        {
            StopCoroutine(_mouseOverCoroutine);
        }
        _timeSlider.fillAmount = 0;
    }
}
