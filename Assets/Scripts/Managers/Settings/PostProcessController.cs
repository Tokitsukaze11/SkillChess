using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    public Volume _globalVolume;
    
    private DepthOfField _depthOfField;

    private bool _isDepthOfField = true;
    private void Awake()
    {
        // TODO : Load setting data
        InitPostProcess();
    }
    private void InitPostProcess()
    {
        _globalVolume.profile.TryGet(out DepthOfField depthOfField);
        _depthOfField = depthOfField;
    }
    public void SetDepthOfField(bool value)
    {
        _isDepthOfField = value;
    }
    public void ControllerDepthOfField(bool value)
    {
        return;
        if (!_isDepthOfField)
            return;
        _depthOfField.active = value;
        // Mode change it seems not supported in this version
        //_depthOfField.mode = new DepthOfFieldModeParameter(value ? DepthOfFieldMode.Gaussian : DepthOfFieldMode.Off);
    }
}
