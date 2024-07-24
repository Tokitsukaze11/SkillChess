using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerPawnController : MonoBehaviour
{
    public GameObject[] playerPawnPrefab;
    public GameObject playerKingPrefab;
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
    private List<Pawn> _playerPawns = new List<Pawn>();
    public event Action PlayerTickHandler;
    private void Awake()
    {
        //ObjectManager.Instance.MakePool(playerPawnPrefab, "PlayerPawn");
    }
    private void Start()
    {
        pawnBehaviorUIPanel.SetActive(false);
    }
    public void SpawnPlayerPawn()
    {
        for(int i = 0; i < 5; i++)
        {
            //var obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab, "PlayerPawn", true);
            GameObject obj = null;
            /*if(i != 0)
                obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab[Random.Range(0,playerPawnPrefab.Length)], null, false);
            else
                obj = ObjectManager.Instance.SpawnObject(playerKingPrefab, null, false);*/
            if(i != 4)
                obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab[i], null, false);
            else
                obj = ObjectManager.Instance.SpawnObject(playerKingPrefab, null, false);
            //var obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab, null, false);
            
            int targetColumn = i*GlobalValues.ROW;
            int targetRow = 0;
            int targetIndex = targetRow + targetColumn;
            var curMapSquare = SquareCalculator.CurrentMapSquare(targetIndex);
            Vector2 curKey = SquareCalculator.CurrentKey(targetIndex);
            
            obj.transform.position = new Vector3(curKey.x, 0, curKey.y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"PlayerPawn_{i}";
            obj.SetActive(true);
            var pawn = obj.GetComponent<Pawn>();
            pawn._isPlayerPawn = true;
            pawn.OnPawnClicked += PawnBehaviorUIPanelActive;
            pawn.OnCannotAction += ButtonShake;
            curMapSquare.CurPawn = pawn;
            pawn.CurMapSquare = curMapSquare;
            pawn.OnDie += PawnDie;
            _playerPawns.Add(pawn);
            pawn._isCanClick = true; // TODO : If random player turn, change this
        }
    }
    public void DespawnPlayerPawn()
    {
        _playerPawns.ForEach(x => ObjectManager.Instance.RemoveObject(x.gameObject, "PlayerPawn", true));
        _playerPawns.Clear();
    }
    public List<Pawn> GetPawns()
    {
        return _playerPawns;
    }
    public void TurnChange(bool isPlayerTurn)
    {
        _playerPawns.ForEach(x => x._isCanClick = isPlayerTurn);
        if(isPlayerTurn)
            PlayerTickHandler?.Invoke();
    }
    private void PawnDie(Pawn diedPawn)
    {
        //ObjectManager.Instance.RemoveObject(diedPawn.gameObject, "PlayerPawn", true);
        _playerPawns.Remove(diedPawn);
        if(diedPawn.PawnType == PawnType.King)
        {
            Debug.Log("Game Over, Enemy Win");
            // TODO : If pawn is king, game over
        }
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject);
    }
    private void PawnBehaviorUIPanelActive(bool active, Pawn curPawn = null)
    {
        if(active)
            _playerPawns.Where(x => x != curPawn).ToList().ForEach(x => x.UnSelected());
        Action uiAction = () =>
        {
            pawnBehaviorUIPanel.SetActive(active);
            if(active)
                BehaviorButtonHandle(curPawn);
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
    private void ButtonShake(int index)
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
