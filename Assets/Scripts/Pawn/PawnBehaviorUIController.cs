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
    public RectTransform _buttonsContainer;
    private List<Pawn> _playerPawns;
    private void Start()
    {
        pawnBehaviorUIPanel.SetActive(false);
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
            _buttonsContainer.DOAnchorPosY(active ? 0 : -200, 0.5f).SetEase(active ? Ease.OutBack : Ease.InBack)
                .onComplete += () =>{if(!active) pawnBehaviorUIPanel.SetActive(false);};
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
