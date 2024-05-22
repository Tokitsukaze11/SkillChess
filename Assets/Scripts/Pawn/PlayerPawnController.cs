using System;
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
    private void Awake()
    {
        ObjectManager.Instance.MakePool(playerPawnPrefab, "PlayerPawn");
    }
    private void Start()
    {
        pawnBehaviorUIPanel.SetActive(false);
    }
    public void SpawnPlayerPawn(Vector2[] spawnPoints)
    {
        for(int i = 0; i < 3; i++)
        {
            var obj = ObjectManager.Instance.SpawnObject(playerPawnPrefab, "PlayerPawn", true);
            obj.transform.position = new Vector3(spawnPoints[i*8].x, 1, spawnPoints[0].y);
            obj.transform.SetParent(ObjectManager.Instance.globalObjectParent);
            obj.gameObject.name = $"PlayerPawn_{i}";
            var pawn = obj.GetComponent<SamplePawn>();
            pawn._isPlayerPawn = true;
            pawn.OnPawnClicked += PawnBehaviorUIPanelActive;
            var curMapSquare = PawnManager.Instance.GetCurrentMapSquare(new Vector2(spawnPoints[i*8].x, spawnPoints[0].y));
            curMapSquare.CurPawn = pawn;
            pawn.CurMapSquare = curMapSquare;
            pawn.OnDie += PawnDie;
        }
    }
    public void DespawnPlayerPawn()
    {
        
    }
    private void PawnDie(Pawn diedPawn)
    {
        ObjectManager.Instance.RemoveObject(diedPawn.gameObject, "PlayerPawn",true);
        // TODO : If pawn is king, game over
    }
    private void PawnBehaviorUIPanelActive(bool active, Pawn curPawn = null)
    {
        pawnBehaviorUIPanel.SetActive(active);
        if(active)
            BehaviorButtonHandle(curPawn);
    }
    private void BehaviorButtonHandle(Pawn curPawn)
    {
        if(curPawn == null)
            throw new System.Exception("When UI is active, curPawn must not be null");
        moveButton.onClick.RemoveAllListeners();
        moveButton.onClick.AddListener(curPawn.ShowMoveRange);
        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(curPawn.ShowAttackRange);
    }
}
