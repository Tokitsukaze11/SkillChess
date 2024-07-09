using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class HealText : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private TextMeshPro _healText;
    private void Awake()
    {
        _mainCamera = GameManager.Instance.mainCamera;
    }
    public void SetText(int heal, Vector3 spawnTrans, bool isCritical = false)
    {
        _healText.text = heal.ToString();
        spawnTrans.y += 1; // Set higher than pawn
        this.transform.position = spawnTrans;
        this.gameObject.SetActive(true);
        StartCoroutine(Co_UpdateRotation());
        this.transform.DOMoveY(spawnTrans.y + 0.3f, 0.5f).OnComplete(() =>
        {
            StopAllCoroutines();
            ObjectManager.Instance.RemoveObject(this.gameObject, StringKeys.HEAL, true);
        });
    }
    public void SetColour(Color color)
    {
        _healText.color = color;
    }
    private IEnumerator Co_UpdateRotation()
    {
        while (true)
        {
            var camRotation = _mainCamera.transform.rotation.eulerAngles;
            this.transform.rotation = Quaternion.Euler(camRotation.x, camRotation.y, camRotation.z);
            yield return null;
        }
    }
}
