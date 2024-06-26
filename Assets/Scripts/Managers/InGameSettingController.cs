using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameSettingController : MonoBehaviour
{
    [Header("InGameMenu")]
    public GameObject _inGameMenuPanel;
    public Button _inGameMenuButton;
    public Button _inGameMenuCloseButton;
    [SerializeField] Image _panelBackImage;
    public Button _cameraResetButton;
    public RectTransform _camResetDesc;
    public TextMeshProUGUI _camResetDescText;
    public Button _surrenderButton;
    public Button _settingButton;
    
    [Header("Scripts")]
    public MapViewController _mapViewController;
    public SettingManager _settingManager;
    public KeyManager _keyManager;
    public PostProcessController _postProcessController;
    
    private event Action<GameState> _onGameStateChange;
    
    private Coroutine _panelAnimCoroutine = null;
    
    private List<RectTransform> _buttonsRect = new List<RectTransform>();
    private List<TextMeshProUGUI> _buttonsText = new List<TextMeshProUGUI>();
    private const int CAMERA_RESET_BUTTON = 0;
    private const int SURRENDER_BUTTON = 1;
    private const int SETTING_BUTTON = 2;
    
    private bool _currentPanelActive = false;
    
    private void Awake()
    {
        _keyManager.AttachKeyEvent(KeyCode.Escape, MenuUpInGame);
        UnLockController.OnKeyAction += _keyManager.AttachKeyEvent;
        _cameraResetButton.GetComponent<PopupObject>().OnMouseOverPopup += CameraResetDescription;
        _onGameStateChange += GameManager.Instance.GameStateChange;
        _buttonsRect.Add(_cameraResetButton.GetComponent<RectTransform>());
        _buttonsRect.Add(_surrenderButton.GetComponent<RectTransform>());
        _buttonsRect.Add(_settingButton.GetComponent<RectTransform>());
        _buttonsText.Add(_cameraResetButton.GetComponentInChildren<TextMeshProUGUI>());
        _buttonsText.Add(_surrenderButton.GetComponentInChildren<TextMeshProUGUI>());
        _buttonsText.Add(_settingButton.GetComponentInChildren<TextMeshProUGUI>());
    }
    private void Start()
    {
        InGameMenuInitialize();
    }
    private void InGameMenuInitialize()
    {
        _inGameMenuButton.onClick.AddListener(() => InGameMenuPanelActive(true));
        _inGameMenuCloseButton.onClick.AddListener(() => InGameMenuPanelActive(false));
        _inGameMenuPanel.SetActive(false);
        _panelBackImage.color = new Color(255,255,255,0);
        _cameraResetButton.onClick.AddListener(CameraReset);
        _surrenderButton.onClick.AddListener(Surrender);
        _camResetDesc.gameObject.SetActive(false);
        _camResetDesc.sizeDelta = new Vector2(600, 0);
        _buttonsRect[CAMERA_RESET_BUTTON].sizeDelta = new Vector2(300, 0);
        _buttonsRect[SURRENDER_BUTTON].sizeDelta = new Vector2(300, 0);
        _buttonsRect[SETTING_BUTTON].sizeDelta = new Vector2(300, 0);
        var curColor = _buttonsText[CAMERA_RESET_BUTTON].color;
        curColor.a = 0;
        _buttonsText[CAMERA_RESET_BUTTON].color = curColor;
        _buttonsText[SURRENDER_BUTTON].color = curColor;
        _buttonsText[SETTING_BUTTON].color = curColor;
        _settingButton.onClick.AddListener(() => SettingPanelActive(true));
    }
    private void InGameMenuPanelActive(bool isActive)
    {
        if(isActive) _inGameMenuPanel.SetActive(true);
        _currentPanelActive = isActive;
        _postProcessController.ControllerDepthOfField(isActive);
        _onGameStateChange!.Invoke(isActive ? GameState.Pause : GameState.Play);
        //_inGameMenuPanel.SetActive(isActive);
        //_panelBackImage.DOFade(isActive ? 0.9f : 0, 0.5f).onComplete += () => _inGameMenuPanel.SetActive(isActive);
        _panelBackImage.DOColor(isActive ? new Color(255, 255, 255, 0.9f) : new Color(255,255,255,0), 0.5f).onComplete += () => _inGameMenuPanel.SetActive(isActive);
        _buttonsRect[CAMERA_RESET_BUTTON].DOSizeDelta(isActive ? new Vector2(300,100) : new Vector2(300,0), 0.5f);
        _buttonsRect[SURRENDER_BUTTON].DOSizeDelta(isActive ? new Vector2(300,100) : new Vector2(300,0), 0.5f);
        _buttonsRect[SETTING_BUTTON].DOSizeDelta(isActive ? new Vector2(300,100) : new Vector2(300,0), 0.5f);
        _buttonsText[CAMERA_RESET_BUTTON].DOFade(isActive ? 1 : 0, 0.3f);
        _buttonsText[SURRENDER_BUTTON].DOFade(isActive ? 1 : 0, 0.3f);
        _buttonsText[SETTING_BUTTON].DOFade(isActive ? 1 : 0, 0.3f);
    }
    private void MenuUpInGame()
    {
        InGameMenuPanelActive(!_currentPanelActive);
    }
    private void CameraReset()
    {
        _mapViewController.CameraReset();
    }
    private void Surrender()
    {
        // TODO : Surrender
        Debug.Log("Surrender");
    }
    private void SettingPanelActive(bool isActive)
    {
        Action UIAction = () =>
        {
            if (isActive)
            {
                _settingManager.ShowSettingPanelFromOther(true);
                _inGameMenuPanel.SetActive(false);
            }
            else
            {
                _inGameMenuPanel.SetActive(true);
                _settingManager.ShowSettingPanelFromOther(false);
            }
        };
        UIManager.Instance.UpdateUI(UIAction);
    }
    private void CameraResetDescription(bool isOn, string description = null)
    {
        _camResetDescText.text = isOn ? description : "";

        Action UIAction = () =>
        {
            Coroutine panelAnimCoroutine = null;
            if (isOn)
            {
                if (_panelAnimCoroutine != null)
                {
                    StopCoroutine(_panelAnimCoroutine);
                }
                _camResetDesc.gameObject.SetActive(true);
                panelAnimCoroutine = StartCoroutine(Co_PanelAnim(true));
            }
            else
            {
                if (_panelAnimCoroutine != null)
                {
                    StopCoroutine(_panelAnimCoroutine);
                }
                panelAnimCoroutine = StartCoroutine(Co_PanelAnim(false));
            }
            _panelAnimCoroutine = panelAnimCoroutine;
        };
        UIManager.Instance.UpdateUI(UIAction);
    }
    private IEnumerator Co_PanelAnim(bool isOn)
    {
        // 목표 패널 크기 설정
        Vector2 targetSize = isOn ? new Vector2(600, 100) : new Vector2(600, 0);

        // 애니메이션 시간 계산
        int frame = GameManager.Instance.TargetFPS;
        float duration = CalculateDuration(isOn, frame);

        float time = 0f;
        float textAnimTime = 0f;

        bool shouldAnimateText = false;

        // 패널 애니메이션
        while (true)
        {
            time += Time.deltaTime;
            _camResetDesc.sizeDelta = Vector2.Lerp(_camResetDesc.sizeDelta, targetSize, time / duration);

            // 패널 애니메이션이 끝나면
            if (Vector2.Distance(_camResetDesc.sizeDelta, targetSize) < 0.1f)
            {
                _camResetDesc.sizeDelta = targetSize;

                // 텍스트 애니메이션 필요 여부 설정
                shouldAnimateText = isOn;
                break;
            }

            yield return null;
        }

        // 텍스트 애니메이션
        if (shouldAnimateText)
        {
            float textDuration = 1f * (GameManager.Instance.TargetFPS / 60f);

            while (true)
            {
                textAnimTime += Time.deltaTime;
                _camResetDescText.color = Color.Lerp(_camResetDescText.color, Color.white, textAnimTime / textDuration);

                if (Vector4.Distance(_camResetDescText.color, Color.white) < 0.1f)
                {
                    _camResetDescText.color = Color.white;
                    break;
                }

                yield return null;
            }
        }
        else
        {
            DeactivatePanel();
        }
    }
    private float CalculateDuration(bool isOn, int frame)
    {
        return isOn ? 2f * (frame / 60f) : 1f * (frame / 60f);
    }
    private void DeactivatePanel()
    {
        _camResetDesc.gameObject.SetActive(false);
        _camResetDescText.color = Color.clear;
    }
}
