using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject _menuPanel;
    public Button _menuButton;
    public Button _closeButton;
    
    [Header("UI Elements")]
    [SerializeField] Image _panelBackImage;

    private void Awake()
    {
        _menuButton.onClick.AddListener(() => MenuPanelActive(true));
        _closeButton.onClick.AddListener(() => MenuPanelActive(false));
    }
    private void Start()
    {
        _menuPanel.SetActive(false);
        _panelBackImage.color = Color.clear;
    }
    private void MenuPanelActive(bool active)
    {
        _menuPanel.SetActive(active);
        _panelBackImage.DOFade(active ? 0.5f : 0, 0.5f);
    }
}
