using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LinkEventPopup : MonoBehaviour
{
    [SerializeField] private RectTransform _popupRect;
    [SerializeField] private TextMeshProUGUI _popupText;
    private const int FONT_SIZE = 30;
    public void Init(string description, Vector3 position)
    {
        _popupText.text = description;
        int textLength = description.Length;
        _popupRect.sizeDelta = new Vector2(textLength * FONT_SIZE, FONT_SIZE * 2);
        _popupRect.position = Vector3.zero;
        _popupRect.anchoredPosition = new Vector2(position.x, position.y);
        _popupRect.localPosition = new Vector3(_popupRect.localPosition.x, _popupRect.localPosition.y, 0);
    }
    public void UpdatePosition(Vector3 position)
    {
        _popupRect.position = position;
    }
}
