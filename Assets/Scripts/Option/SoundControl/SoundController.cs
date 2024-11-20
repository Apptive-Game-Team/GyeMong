using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : SingletonObject<SoundController>
{
    public Scrollbar masterVolumeSlider;
    public Scrollbar uiVolumeSlider;
    public Scrollbar bgmVolumeSlider;
    public Scrollbar effectVolumeSlider;

    private void Start()
    {
        InitialSetting();
    }

    private void InitialSetting()
    {
        masterVolumeSlider = transform.Find("MasterVolumeSlider").GetComponent<Scrollbar>();
        uiVolumeSlider = transform.Find("UIVolumeSlider").GetComponent<Scrollbar>();
        bgmVolumeSlider = transform.Find("BgmVolumeSlider").GetComponent<Scrollbar>();
        effectVolumeSlider = transform.Find("EffectVolumeSlider").GetComponent<Scrollbar>();

        SoundData soundData = DataManager.Instance.LoadSection<SoundData>("SoundData");
        masterVolumeSlider.value = soundData.masterVolume;
        uiVolumeSlider.value = soundData.UIVolume;
        bgmVolumeSlider.value = soundData.bgmVolume;
        effectVolumeSlider.value = soundData.sfxVolume;

        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        uiVolumeSlider.onValueChanged.AddListener(UpdateUIVolume);
        bgmVolumeSlider.onValueChanged.AddListener(UpdateBgmVolume);
        effectVolumeSlider.onValueChanged.AddListener(UpdateEffectVolume);
    }

    private void UpdateMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    private void UpdateUIVolume(float value)
    {
        SoundManager.Instance.SetVolume(SoundType.UI, value);
    }

    private void UpdateBgmVolume(float value)
    {
        SoundManager.Instance.SetVolume(SoundType.BGM, value);
    }

    private void UpdateEffectVolume(float value)
    {
        SoundManager.Instance.SetVolume(SoundType.EFFECT, value);
    }
}
