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
    private int _pawnLayMask;
    private MapSquare _mouseOverSquare = null;
    [SerializeField] private ObstacleController _obstacleController;
    private List<Obstacle> _preObstacles = new List<Obstacle>();
    private bool _isCanClick = false;
    private void Awake()
    {
        _mainCamera = GameManager.Instance.mainCamera;
        //UpdateManager.Instance.OnUpdate += OnMouseClick;
        //Observable.EveryGameObjectUpdate().Subscribe(_ => OnMouseClick());
        //this.UpdateAsObservable().Subscribe(_ => OnMouseClick());
        _squareLayMask = LayerMask.GetMask("Square");
        _pawnLayMask = LayerMask.GetMask("Pawn");

        var mouseDownStream = this.UpdateAsObservable().Where(x => Input.GetMouseButtonDown((0)));
        mouseDownStream.Subscribe(_ => OnMouseClick());
        
        var updateStream = this.UpdateAsObservable();
        updateStream.Subscribe(_ => MouseOverDetector());

        /*GameManager.Instance.OnGameStart += () =>
        {
            _isCanClick = true;
        };*/
        EventManager.Instance.OnGameStart += () =>
        {
            _isCanClick = true;
        };
        GameManager.Instance.OnGameEnd += (x) =>
        {
            _isCanClick = false;
        };
    }
    private void OnMouseClick()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;
        if (!_isCanClick)
            return;
        if (ReferenceEquals(_mouseOverSquare, null))
            return;
        _mouseOverSquare.OnMouseClick(TryPawnClick);
    }
    private void TryPawnClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var pawn = RaycastTool.RaycastOnLayer<Pawn>(ray, _pawnLayMask);
        pawn?.OnMouseClick();
    }
    private void MouseOverDetector()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        // 레이케스트에서는 하나의 MapSquare만 인식이 가능함(님은 2개 동시에 클릭 ㄱㄴ?)
        var mapSquare = RaycastTool.RaycastOnLayer<MapSquare>(ray, _squareLayMask);
        if (ReferenceEquals(mapSquare, null))
        {
            if (CheckOtherSquare()) // Exit Event
                _mouseOverSquare = null;
            // Nothing Hit
            return;
        }
        if (ReferenceEquals(mapSquare, _mouseOverSquare)) // Same Square
            return;
        // Enter Event
        CheckOtherSquare();
        _mouseOverSquare = mapSquare;
        Action obstacleCheck = () =>
        {
            var origin = _mainCamera.transform.position;
            var distance = _mouseOverSquare.transform.position - _mainCamera.transform.position;
            var obs = RaycastTool.RaycastNonAlloc<Obstacle>(origin,distance,new RaycastHit[10]);
            foreach (var obstacle in obs)
            {
                _obstacleController.SetPreCachedObstacles(obstacle);
            }
            _preObstacles = obs;
        };
        _mouseOverSquare.OnMouseEnterCast(obstacleCheck);
    }
    private bool CheckOtherSquare()
    {
        if(ReferenceEquals(_mouseOverSquare, null))
            return false;
        Action obstacleRemove = () =>
        {
            foreach (var obstacle in _preObstacles)
            {
                _obstacleController.RemovePreCachedObstacles(obstacle);
            }
            _preObstacles.Clear();
        };
        _mouseOverSquare.OnMouseExitCast(obstacleRemove);
        return true;
    }
}
