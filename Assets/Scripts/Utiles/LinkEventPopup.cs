using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LinkEventPopup : MonoBehaviour
{
    [SerializeField] private RectTransform _popupRect;
    [SerializeField] private TextMeshProUGUI _popupText;
    private const int FONT_HEIGHT = 50;
    private const int FONT_WIDTH = 30;
    private const int MAX_WIDTH = 1000;
    public void Init(string description, Vector3 position)
    {
        _popupText.text = description;
        // 가로 길이 계산
        int[] descriptionLength = description.Split('\n').Select(x => x.Length).ToArray();
        int max = descriptionLength.Max();
        int width = max * FONT_WIDTH > MAX_WIDTH ? MAX_WIDTH : max * FONT_WIDTH;
        // 세로 길이 계산
        int paragraph = description.Count(x => x == '\n') + 1;
        
        var longestParagraph = description.Split('\n').OrderByDescending(x => x.Length).First();
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(longestParagraph);
        int fullWidthCount = longestParagraph.Count(c => c > '\u1FFF');
        int halfWidthCount = longestParagraph.Length - fullWidthCount;
        // 전제 : 가로 길이 1000 기준 약 37글자의 전각 문자가 들어감.
        // 목표 : 가로 길이 1000에 비해 얼마나 긴지 계산하여 세로 길이를 조절
        // 모든 글자를 전각 기준으로 바꾸어 몇 글짜인지 체크
        int totalCharCount = fullWidthCount + (halfWidthCount/2);
        // 전각과 반각을 전부 더해서 가로 길이 1000에 비해 얼마나 긴지 계산
        int ratio = totalCharCount / 37;
        // 가로 길이 1000에 비해 얼마나 긴지 계산한 비율을 세로 길이에 적용
        paragraph += ratio;
        // 최종 세로 길이 계산
        int height = FONT_HEIGHT * paragraph;
        // 계산된 가로 세로 길이로 팝업 크기 조정
        _popupRect.sizeDelta = new Vector2(width, height);
        
        position.y += height * 0.5f; // 팝업이 마우스 보다 위로 나타나도록 위치 조정
        
        if (position.x + width * 0.5f > Screen.width) // 팝업이 화면을 넘어가지 않도록 보정
        {
            position.x = Screen.width - width * 0.5f;
        }
        _popupRect.position = Vector3.zero;
        _popupRect.anchoredPosition = new Vector2(position.x, position.y);
        _popupRect.localPosition = new Vector3(_popupRect.localPosition.x, _popupRect.localPosition.y, 0);
    }
    public void UpdatePosition(Vector3 position)
    {
        _popupRect.position = position;
    }
}
