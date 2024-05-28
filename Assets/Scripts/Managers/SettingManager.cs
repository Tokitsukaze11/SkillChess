using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject _menuPanel;
    public Button _menuButton;

    private void Awake()
    {
        _menuButton.onClick.AddListener(MenuControl);
    }
    private void Start()
    {
        UIManager.Instance.UpdateUI(()=>MenuPanelActive(false));
    }
    private void MenuControl()
    {
        bool isActive = _menuPanel.activeSelf;
        UIManager.Instance.UpdateUI(()=>MenuPanelActive(!isActive));
    }
    private void MenuPanelActive(bool active)
    {
        _menuPanel.SetActive(active);
    }
}
