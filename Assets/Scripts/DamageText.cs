using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private TextMeshPro _damageText;
    private void Awake()
    {
        _mainCamera = GameManager.Instance.mainCamera;
    }
    public void SetText(int damage, Vector3 spawnTrans, bool isCritical = false)
    {
        _damageText.text = damage.ToString();
        spawnTrans.y += 1; // Set higher than pawn
        this.transform.position = spawnTrans;
        this.gameObject.SetActive(true);
        StartCoroutine(Co_UpdateRotation());
        this.transform.DOMoveY(spawnTrans.y + 1, 1f).OnComplete(() =>
        {
            StopAllCoroutines();
            ObjectManager.Instance.RemoveObject(this.gameObject, StringKeys.DAMAGE, true);
        });
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
