using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewController : MonoBehaviour
{
    public GameObject _mainCamera;
    [SerializeField] private float _cameraSpeed = 10; // 10~30 is best
    [SerializeField] private int _interval = 1;
    private KeyCode _rightRotationKey = KeyCode.E;
    private KeyCode _leftRotationKey = KeyCode.Q;
    private Transform _cameraTransform;
    //private Vector3 _targetPosition;
    private void Awake()
    {
        _cameraTransform = _mainCamera.transform;
        UpdateManager.Instance.OnUpdate += KeyCheck;
    }
    private void InitCameraTarget()
    {
        int n = 8;
        int m = 8;
        float x = 1.5f;
        //_targetPosition = new Vector3((n / 2f) * x, 0, (m / 2f) * x);
        //_mainCamera.transform.LookAt(_targetPosition);
    }
    private void KeyCheck()
    {
        /*HorizontalMove();
        VerticalMove();*/
        CameraMove();
        HeightMove();
        RotationMove();
    }
    private void CameraMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 localMoveVector = new Vector3(horizontal, 0, vertical);
        
        float thetaX = transform.rotation.eulerAngles.x * Mathf.Deg2Rad; // x축 회전각
        float thetaY = transform.rotation.eulerAngles.y * Mathf.Deg2Rad; // y축 회전각 (좌우 회전)
        var rotationMatrix = GetRotationMatrix(thetaX, thetaY);
        
        Vector3 globalMoveVector = rotationMatrix * localMoveVector;
        _cameraTransform.position += globalMoveVector * _cameraSpeed / _interval * Time.deltaTime;
    }
    private void HorizontalMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal == 0)
            return;
        float thetaX = transform.rotation.eulerAngles.x * Mathf.Deg2Rad; // x축 회전각
        float thetaY = transform.rotation.eulerAngles.y * Mathf.Deg2Rad; // y축 회전각 (좌우 회전)
        var rotationMatrix = GetRotationMatrix(thetaX, thetaY);
        Vector3 globalMoveVector = rotationMatrix * new Vector3(horizontal, 0, 0);
        _cameraTransform.position += globalMoveVector * _cameraSpeed / _interval * Time.deltaTime;
    }
    private void VerticalMove()
    {
        float vertical = Input.GetAxis("Vertical");
        if (vertical == 0)
            return;
        float thetaX = transform.rotation.eulerAngles.x * Mathf.Deg2Rad; // x축 회전각
        float thetaY = transform.rotation.eulerAngles.y * Mathf.Deg2Rad; // y축 회전각 (좌우 회전)
        var rotationMatrix = GetRotationMatrix(thetaX, thetaY);
        Vector3 globalMoveVector = rotationMatrix * new Vector3(0, 0, vertical);
        _cameraTransform.position += globalMoveVector * _cameraSpeed / _interval * Time.deltaTime;
    }
    private Matrix4x4 GetRotationMatrix(float thetaX, float thetaY)
    {
        Matrix4x4 rotationMatrixX = new Matrix4x4();
        rotationMatrixX.SetRow(0, new Vector4(1, 0, 0, 0));
        rotationMatrixX.SetRow(1, new Vector4(0, Mathf.Cos(thetaX), -Mathf.Sin(thetaX), 0));
        rotationMatrixX.SetRow(2, new Vector4(0, Mathf.Sin(thetaX), Mathf.Cos(thetaX), 0));
        rotationMatrixX.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 rotationMatrixY = new Matrix4x4();
        rotationMatrixY.SetRow(0, new Vector4(Mathf.Cos(thetaY), 0, Mathf.Sin(thetaY), 0));
        rotationMatrixY.SetRow(1, new Vector4(0, 1, 0, 0));
        rotationMatrixY.SetRow(2, new Vector4(-Mathf.Sin(thetaY), 0, Mathf.Cos(thetaY), 0));
        rotationMatrixY.SetRow(3, new Vector4(0, 0, 0, 1));

        return rotationMatrixY * rotationMatrixX;
    }
    private void HeightMove()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            _cameraTransform.position += new Vector3(0, -scroll * 10 * Time.deltaTime, 0);
        }
    }
    private void RotationMove()
    {
        // Y축 회전
        if (Input.GetKey(_rightRotationKey))
            _cameraTransform.Rotate(Vector3.up, _cameraSpeed * Time.deltaTime,Space.World);
        if (Input.GetKey(_leftRotationKey))
            _cameraTransform.Rotate(Vector3.up, -_cameraSpeed * Time.deltaTime,Space.World);
    }
}
