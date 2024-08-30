using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [Header("Title UI")]
    public GameObject _titlePanel;
    public Image _titleBG;
    public GameObject _titleText;
    public Button _startButton;
    public Button _exitButton;
    public GameObject _mapSetPanel;
    private void Awake()
    {
        _startButton.onClick.AddListener(StartGame);
        _exitButton.onClick.AddListener(ExitGame);
        GameManager.Instance.OnTitle += () => TitleChange(true);
    }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        _titlePanel.SetActive(true);
        var originColor = _titleBG.color;
        originColor.a = 1;
        _titleBG.color = originColor;
        _titleText.transform.localScale = Vector3.one;
        _mapSetPanel.transform.localScale = Vector3.one;
        _startButton.transform.localScale = Vector3.one;
        _exitButton.transform.localScale = Vector3.one;
    }
    private void StartGame()
    {
        TitleChange(false);
        //EventManager.Instance.GameStart(GlobalValues.ROW, GlobalValues.COL);
        GameManager.Instance.GameStart();
    }
    private void ExitGame()
    {
        GameManager.Instance.CloseGame();
    }
    private void TitleChange(bool isOn)
    {
        if(isOn)
            _titlePanel.SetActive(true);
        _titleBG.DOFade(isOn ? 1 : 0, 0.5f);
        _titleText.transform.DOScale(isOn ? Vector3.one : Vector3.zero, 0.5f);
        _mapSetPanel.transform.DOScale(isOn ? Vector3.one : Vector3.zero, 0.5f);
        _startButton.transform.DOScale(isOn ? Vector3.one : Vector3.zero, 0.5f);
        _exitButton.transform.DOScale(isOn ? Vector3.one : Vector3.zero, 0.5f).onComplete += () =>
        {
            if (!isOn)
                _titlePanel.SetActive(false);
        };
    }
}
