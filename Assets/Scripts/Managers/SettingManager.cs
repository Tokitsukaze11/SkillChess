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
    [Header("InGameMenu")]
    public GameObject _inGameMenuPanel;
    public Button _inGameMenuButton;
    public Button _inGameMenuCloseButton;
    [SerializeField] Image _panelBackImage;
    public Button _cameraResetButton;
    public RectTransform _camResetDesc;
    public TextMeshProUGUI _camResetDescText;
    private Coroutine _panelAnimCoroutine = null;
    
    [Header("Scripts")]
    public MapViewController _mapViewController;
    
    private Action<GameState> _onGameStateChange;

    private void Awake()
    {
        UpdateManager.Instance.OnUpdate += MenuUpInGame;
        _cameraResetButton.GetComponent<PopupObject>().OnMouseOverPopup += CameraResetDescription;
        _onGameStateChange += GameManager.Instance.GameStateChange;
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
        _panelBackImage.color = Color.clear;
        _cameraResetButton.onClick.AddListener(CameraReset);
        _camResetDesc.gameObject.SetActive(false);
        _camResetDesc.sizeDelta = new Vector2(600, 0);
    }
    private void InGameMenuPanelActive(bool isActive)
    {
        _onGameStateChange.Invoke(isActive ? GameState.Pause : GameState.Play);
        _inGameMenuPanel.SetActive(isActive);
        _panelBackImage.DOFade(isActive ? 0.5f : 0, 0.5f);
    }
    private void MenuUpInGame()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
            InGameMenuPanelActive(true);
    }
    private void CameraReset()
    {
        _mapViewController.CameraReset();
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
    /*private IEnumerator Co_PanelAnim(bool isOn)
    {
        Vector2 targetSize = isOn ? new Vector2(600, 100) : new Vector2(600, 0);

        float time = 0;
        int frame = GameManager.Instance.TargetFPS;
        float duration = isOn ? 2f * (frame/60f) : 1f * (frame/60f);

        while (true)
        {
            time += Time.deltaTime;
            _camResetDesc.sizeDelta = Vector2.Lerp(_camResetDesc.sizeDelta, targetSize, time/duration);
            if (Vector2.Distance(_camResetDesc.sizeDelta, targetSize) < 0.1f)
            {
                _camResetDesc.sizeDelta = targetSize;
                if (!isOn)
                {
                    _camResetDesc.gameObject.SetActive(false);
                    _camResetDescText.color = Color.clear;
                    yield break;
                }
                float time2 = 0;
                float duration2 = 1f * (frame/60f);
                while (true)
                {
                    time2 += Time.deltaTime;
                    _camResetDescText.color = Color.Lerp(_camResetDescText.color, Color.white, time2/duration2);
                    if (Vector4.Distance(_camResetDescText.color, Color.white) < 0.1f)
                    {
                        _camResetDescText.color = Color.white;
                        yield break;
                    }
                    yield return null;
                }
            }
            yield return null;
        }
    }*/
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
