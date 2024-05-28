using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private void Awake()
    {
        _movePopup.OnMouseOverPopup += PopupControl;
        _attackPopup.OnMouseOverPopup += PopupControl;
        _defendPopup.OnMouseOverPopup += PopupControl;
        _skillPopup.OnMouseOverPopup += PopupControl;
    }
    private void Start()
    {
        PopupControl(false);
    }
    private void PopupControl(bool isOn,string description = null)
    {
        _descriptionText.text = isOn ? description : "";

        Action UIAction = () =>
        {
            if (isOn)
            {
                _descriptionPopupImage.gameObject.SetActive(true);
                _descriptionPopupImage.rectTransform.DOSizeDelta(new Vector2(800, 230), 0.5f);
            }
            else
                _descriptionPopupImage.rectTransform.DOSizeDelta(new Vector2(800, 0), 0.1f).OnComplete(
                    () => _descriptionPopupImage.gameObject.SetActive(false));
        };
        UIManager.Instance.UpdateUI(UIAction);
    }
}
