using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPawnController : MonoBehaviour
{
    public GameObject playerPawnPrefab;
    [Header("Pawn Behavior UI Elements")]
    public GameObject pawnBehaviorUIPanel;
    public Button moveButton;
    public Button attackButton;
    public Button defendButton;
    public Button skillButton;
    private List<Pawn> _playerPawns = new List<Pawn>();
    public event Action PlayerTickHandler;
    private void Awake()
    {
        ObjectManager.Instance.MakePool(playerPawnPrefab, "PlayerPawn");
    }
    private void Start()
    {
        pawnBehaviorUIPanel.SetActive(false);
    }
    public void SpawnPlayerPawn()
    {
        for(int i = 0; i < 3; i++)
        {
            var obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab, "PlayerPawn", true);
            
            int targetColumn = i*GlobalValues.ROW;
            int targetRow = 0;
            int targetIndex = targetRow + targetColumn;
            var curMapSquare = SquareCalculator.CurrentMapSquare(targetIndex);
            Vector2 curKey = SquareCalculator.CurrentKey(targetIndex);
            
            obj.transform.position = new Vector3(curKey.x, 1, curKey.y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"PlayerPawn_{i}";
            var pawn = obj.GetComponent<SamplePawn>();
            pawn._isPlayerPawn = true;
            pawn.OnPawnClicked += PawnBehaviorUIPanelActive;
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
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject, "PlayerPawn", true);
        _playerPawns.Remove(diedPawn);
        if(diedPawn.PawnType == PawnType.King)
        {
            Debug.Log("Game Over");
            // TODO : If pawn is king, game over
        }
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
    }
}
