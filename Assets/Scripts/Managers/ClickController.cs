using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ClickController : MonoBehaviour
{
    private Camera _mainCamera;
    private int _squareLayMask;
    private MapSquare _mouseOverSquare = null;
    [SerializeField] private ObstacleController _obstacleController;
    private void Awake()
    {
        _mainCamera = GameManager.Instance.mainCamera;
        //UpdateManager.Instance.OnUpdate += OnMouseClick;
        //Observable.EveryGameObjectUpdate().Subscribe(_ => OnMouseClick());
        //this.UpdateAsObservable().Subscribe(_ => OnMouseClick());
        _squareLayMask = LayerMask.GetMask("Square");

        var mouseDownStream = this.UpdateAsObservable().Where(x => Input.GetMouseButtonDown((0)));
        mouseDownStream.Subscribe(_ => OnMouseClick());
        
        var updateStream = this.UpdateAsObservable();
        updateStream.Subscribe(_ => MouseOverDetector());
    }
    private void OnMouseClick()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;
        // Enter가 먼저 실행되기 때문에 Raycast를 안 쏴도 됨
        _mouseOverSquare?.OnMouseClick();
    }
    private void MouseOverDetector()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        // 레이케스트에서는 하나의 MapSquare만 인식이 가능함(님은 2개 동시에 클릭 ㄱㄴ?)
        RaycastHit hit;
        Physics.Raycast(ray, out hit, float.MaxValue, _squareLayMask);
        if (hit.collider != null)
        {
            hit.collider.TryGetComponent<MapSquare>(out var mapSquare);
            //if (mapSquare == null || mapSquare == _mouseOverSquare)
            if(ReferenceEquals(mapSquare, null) || ReferenceEquals(mapSquare, _mouseOverSquare))
                return;
            // Enter Event
            CheckOtherSquare();
            _mouseOverSquare = mapSquare;
            _mouseOverSquare.OnMouseEnterCast();
            return;
        }
        // Exit Event
        if(CheckOtherSquare())
            _mouseOverSquare = null;
    }
    private bool CheckOtherSquare()
    {
        if(ReferenceEquals(_mouseOverSquare, null))
            return false;
        _mouseOverSquare.OnMouseExitCast();
        return true;
    }
}
