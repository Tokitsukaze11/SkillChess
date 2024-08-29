using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool _isPlayer1Turn = true;
    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI _turnText;
    private RectTransform _turnTextRect;
    private void Awake()
    {
        GameManager.Instance.OnTurnEnd += TurnChange;
        GameManager.Instance.IsPlayer1Turn = () => _isPlayer1Turn;
        GameManager.Instance.OnGameRestart += StartGame;
        _turnTextRect = _turnText.gameObject.GetComponent<RectTransform>();
        EventManager.Instance.OnGameStart += (row, col) => StartGame();
    }
    private void StartGame()
    {
        _isPlayer1Turn = true;
        StartCoroutine(Co_TurnChange(true, 5));
    }
    private void TurnChange()
    {
        _isPlayer1Turn = !_isPlayer1Turn;
        Action action = _isPlayer1Turn ? PlayerTurn : EnemyTurn;
        action!.Invoke();
    }
    private void PlayerTurn()
    {
        PawnManager.Instance.TurnChange(true);
        StartCoroutine(Co_TurnChange(true));
    }
    private void EnemyTurn()
    {
        PawnManager.Instance.TurnChange(false);
        StartCoroutine(Co_TurnChange(false));
    }
    private IEnumerator Co_TurnChange(bool isPlayer1, int delay = 0)
    {
        yield return new WaitForSeconds(delay);
        _turnText.gameObject.SetActive(true);
        _turnText.text = isPlayer1 ? "Player1 턴" : "Player2 턴";
        _turnTextRect.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(1f);
        _turnTextRect.DOAnchorPosX(1200, 0.3f).SetEase(Ease.InQuint).onComplete += () =>
        {
            _turnTextRect.anchoredPosition = new Vector2(-1200, -150);
            _turnText.gameObject.SetActive(false);
        };
        yield break;
    }
}
