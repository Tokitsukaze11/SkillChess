using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private Button _endButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Image _panelImage;
    [SerializeField] private GameObject _mapSetPanel;
    private void Awake()
    {
        GameManager.Instance.OnGameEnd += GameEnd;
        _endButton.onClick.AddListener(() =>
        {
            GameManager.Instance.CloseGame();
        });
        _restartButton.onClick.AddListener(RestartGame);
    }
    private void Start()
    {
        _endPanel.SetActive(false);
        _panelImage.color = new Color(118/255f, 118/255f, 118/255f, 0f);
        _winText.rectTransform.localScale = new Vector3(1, 0, 1);
        _endButton.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, -200);
        _restartButton.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150, -200);
        _mapSetPanel.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
    }
    private void GameEnd(bool isPlayerWin)
    {
        _endPanel.SetActive(true);
        _winText.text = isPlayerWin ? "Player1 승리" : "Player2 승리";
        _panelImage.DOFade(1, 0.3f).onComplete += () =>
        {
            _winText.rectTransform.DOScaleY(1, 0.3f);
            _restartButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f);
            _endButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f).SetDelay(0.1f);
            _mapSetPanel.gameObject.GetComponent<RectTransform>().DOScaleY(1, 0.3f);
        };
    }
    private void RestartGame()
    {
        _winText.rectTransform.DOScaleY(0, 0.3f);
        _restartButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-200, 0.3f).SetDelay(0.1f);
        _endButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-200, 0.3f);
        _mapSetPanel.gameObject.GetComponent<RectTransform>().DOScaleY(0, 0.3f);
        _panelImage.DOFade(0, 0.3f).onComplete += () =>
        {
            _endPanel.SetActive(false);
        };
        GameManager.Instance.GameRestart();
    }
}
