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
    [SerializeField] private Image _panelImage;
    private void Awake()
    {
        GameManager.Instance.OnGameEnd += GameEnd;
    }
    private void GameEnd(bool isPlayerWin)
    {
        _endPanel.SetActive(true);
        _winText.text = isPlayerWin ? "Player1 승리" : "Player2 승리";
        _endButton.onClick.AddListener(() =>
        {
            Debug.Log("게임 종료");
        });
    }
    private IEnumerator GameEndCoroutine()
    {
        _panelImage.DOColor(new Color(0, 0, 0, 0.5f), 1f);
        yield break;
    }
}
