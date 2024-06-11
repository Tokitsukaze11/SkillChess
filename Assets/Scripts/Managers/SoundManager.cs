using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private GameObject _SFXContainer;
    [SerializeField] private List<AudioSource> _SFXSources;
    [SerializeField] private AudioSource _BGMSource;
    [SerializeField] private AudioMixer _audioMixer;
    
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;
    
    private float _masterVolume = 1f;
    private float _sfxVolume = 1f;
    private float _bgmVolume = 1f;
    
    private const int MIN_VOLUME = -80;
    private const int MAX_VOLUME = 0;

    private void Awake()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        _bgmSlider.onValueChanged.AddListener(SetBgmVolume);
    }
    private void SetMasterVolume(float volume)
    {
        _masterVolume = volume;
        _audioMixer.SetFloat("Master", Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, _masterVolume));
    }
    private void SetSfxVolume(float volume)
    {
        _sfxVolume = volume;
        _audioMixer.SetFloat("SFX", Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, _sfxVolume));
    }
    private void SetBgmVolume(float volume)
    {
        _bgmVolume = volume;
        _audioMixer.SetFloat("BGM", Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, _bgmVolume));
    }
    public void ChangeBGM(AudioClip clip)
    {
        _BGMSource.clip = clip;
        _BGMSource.Play();
    }
    public void PlaySfx(AudioClip clip)
    {
        ClearSfx();
        foreach (var source in _SFXSources.Where(source => !source.isPlaying))
        {
            source.clip = clip;
            source.Play();
            return;
        }
        AddSfxSource();
        var newSource = _SFXSources.Last();
        newSource.clip = clip;
        newSource.Play();
    }
    private void AddSfxSource()
    {
        var newSource = Instantiate(_SFXSources[0], _SFXContainer.transform);
        _SFXSources.Add(newSource);
    }
    private void ClearSfx()
    {
        if(_SFXSources.Count < 4)
            return;
        _SFXSources.Skip(3).Where(x => !x.isPlaying).ToList().ForEach(x => Destroy(x.gameObject));
        _SFXSources = _SFXSources.Where(x => x != null).ToList();
    }
}
