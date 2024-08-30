using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PawnBehaviorUIController : MonoBehaviour
{
    [Header("Pawn Behavior UI Elements")]
    public GameObject pawnBehaviorUIPanel;
    public Button moveButton;
    [SerializeField] private Image _moveTimer;
    public Button attackButton;
    [SerializeField] private Image _attackTimer;
    public Button defendButton;
    [SerializeField] private Image _defendTimer;
    public Button skillButton;
    [SerializeField] private Image _skillTimer;
    public Image _skillIconImage;
    private List<Pawn> _playerPawns;
    private void Start()
    {
        pawnBehaviorUIPanel.SetActive(false);
        GameManager.Instance.OnTitle += () => PawnBehaviorUIPanelActive(false); // 적용시 에러 발생 => Pawn들이 제거될 때 outlineFX를 제거하도록 임시 조치
        // 근데 계속 에러 발생함(ㅅㅂ?)
        GameManager.Instance.OnGameEnd += (x) => PawnBehaviorUIPanelActive(false); // 얘는 pawn이랑 상관 없음
    }
    public void UpdatePlayerPawns(List<Pawn> playerPawns)
    {
        _playerPawns = playerPawns;
    }
    public void PawnBehaviorUIPanelActive(bool active, Pawn curPawn = null)
    {
        if(active)
            _playerPawns.Where(x => x != curPawn).ToList().ForEach(x => x.UnSelected());
        Action uiAction = () =>
        {
            if(active)
            {
                BehaviorButtonHandle(curPawn);
                pawnBehaviorUIPanel.SetActive(true);
            }
            /*_buttonsContainer.DOAnchorPosY(active ? 0 : -200, 0.5f).SetEase(active ? Ease.OutBack : Ease.InBack)
                .onComplete += () =>{if(!active) pawnBehaviorUIPanel.SetActive(false);};*/
            skillButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(active ? 0 : -200, 0.5f)
                .SetEase(active ? Ease.OutBack : Ease.InBack).SetDelay(active ? 0 : 0.3f)
                .onComplete += () => {if(!active) pawnBehaviorUIPanel.SetActive(false);};
            defendButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(active ? 0 : -200, 0.5f)
                .SetEase(active ? Ease.OutBack : Ease.InBack).SetDelay(active ? 0.1f : 0.2f);
            attackButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(active ? 0 : -200, 0.5f)
                .SetEase(active ? Ease.OutBack : Ease.InBack).SetDelay(active ? 0.2f : 0.1f);
            moveButton.gameObject.GetComponent<RectTransform>().DOAnchorPosY(active ? 0 : -200, 0.5f)
                .SetEase(active ? Ease.OutBack : Ease.InBack).SetDelay(active ? 0.3f : 0);
        };
        UIManager.Instance.UpdateUI(uiAction);
    }
    private void BehaviorButtonHandle(Pawn curPawn)
    {
        if(curPawn == null)
            throw new System.Exception("When UI is active, curPawn must not be null");
        
        moveButton.onClick.RemoveAllListeners();
        moveButton.onClick.AddListener(curPawn.ShowMoveRange);
        moveButton.gameObject.GetComponent<PopupObject>().InitDescription(curPawn._descriptObjects[0]);
        
        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(curPawn.ShowAttackRange);
        attackButton.gameObject.GetComponent<PopupObject>().InitDescription(curPawn._descriptObjects[1]);
        
        defendButton.onClick.RemoveAllListeners();
        defendButton.onClick.AddListener(curPawn.Defend);
        defendButton.gameObject.GetComponent<PopupObject>().InitDescription(curPawn._descriptObjects[2]);
        
        skillButton.onClick.RemoveAllListeners();
        skillButton.onClick.AddListener(curPawn.UseSkill);
        skillButton.gameObject.GetComponent<PopupObject>().InitDescription(curPawn._descriptObjects[3]);
        _skillIconImage.sprite = curPawn._skillImage;
    }
    public void ButtonShake(int index)
    {
        switch(index)
        {
            case 0:
                moveButton.gameObject.GetComponent<PopupObject>().StopFillAnim();
                moveButton.GetComponent<RectTransform>().DOShakePosition(0.5f, 10, 90, 90, false, true);
                _moveTimer.fillAmount = 1;
                _moveTimer.DOFillAmount(0, 0.5f).SetDelay(1f);
                break;
            case 1:
                attackButton.gameObject.GetComponent<PopupObject>().StopFillAnim();
                attackButton.GetComponent<RectTransform>().DOShakePosition(0.5f, 10, 90, 90, false, true);
                _attackTimer.fillAmount = 1;
                _attackTimer.DOFillAmount(0, 0.5f).SetDelay(1f);
                break;
            case 2:
                throw new System.ArgumentException("Defend is not allowed to shake");
                break;
            case 3:
                skillButton.gameObject.GetComponent<PopupObject>().StopFillAnim();
                skillButton.GetComponent<RectTransform>().DOShakePosition(0.5f, 10, 90, 90, false, true);
                _skillTimer.fillAmount = 1;
                _skillTimer.DOFillAmount(0, 0.5f).SetDelay(1f);
                break;
        }
    }
}
