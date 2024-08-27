using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MapViewController : MonoBehaviour
{
    public GameObject _mainCamera;
    [SerializeField] private float _cameraSpeed = 10; // 10~30 is best
    [SerializeField] private int _interval = 1;
    [SerializeField] private float _scrollSpeed = 1000;
    [SerializeField] private float _rotationSpeed = 100;
    private KeyCode _rightRotationKey = KeyCode.E;
    private KeyCode _leftRotationKey = KeyCode.Q;
    private Transform _cameraTransform;
    
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private void Awake()
    {
        _cameraTransform = _mainCamera.transform;
        var updateStream = this.UpdateAsObservable();
        updateStream.Subscribe(_ => KeyCheck());
        
        _defaultPosition = _cameraTransform.position;
        _defaultRotation = _cameraTransform.rotation;
        GameManager.Instance.OnGameRestart += CameraReset;
    }
    private void KeyCheck()
    {
        if(GameManager.Instance.GameState != GameState.Play) return;
        HeightMove();
        RotationMove();
        CameraMove();
    }
    private void CameraMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        var newForward = _cameraTransform.forward;
        float xMove = horizontal * _cameraSpeed / _interval * Time.deltaTime;
        float zMove = vertical * _cameraSpeed / _interval * Time.deltaTime;
        Vector3 moveVector = new Vector3(newForward.x * zMove + newForward.z * xMove, 0, newForward.z * zMove - newForward.x * xMove);
        
        _cameraTransform.position += moveVector;
    }
    private void HeightMove()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            _cameraTransform.position += new Vector3(0, -scroll * _scrollSpeed * Time.deltaTime, 0);
        }
    }
    private void RotationMove()
    {
        // Y축 회전
        if (Input.GetKey(_rightRotationKey))
            _cameraTransform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime,Space.World);
        if (Input.GetKey(_leftRotationKey))
            _cameraTransform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime,Space.World);
    }
    public void CameraReset()
    {
        CamPositionSet();
        _cameraTransform.rotation = _defaultRotation;
    }
    private void CamPositionSet()
    {
        int col = GlobalValues.COL;
        
        var targetX = (col - 8) * (1.5f / 2);
        var newPos = new Vector3(_defaultPosition.x + targetX, _defaultPosition.y, _defaultPosition.z);
        _cameraTransform.position = newPos;
    }
}
