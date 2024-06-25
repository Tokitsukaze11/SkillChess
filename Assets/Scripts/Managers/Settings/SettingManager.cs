using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("SettingPanel")]
    public GameObject _settingPanel;
    public Image _backPanel;
    
    public void ShowSettingPanelFromOther(bool active)
    {
        Debug.Log("Not working now");
        return;
        _settingPanel.SetActive(active);
        _backPanel.DOColor(active ? new Color(0, 0, 0, 0.9f) : Color.clear, 0.5f);
    }
    public void SetSettingPanel(bool active)
    {
        
    }
}
