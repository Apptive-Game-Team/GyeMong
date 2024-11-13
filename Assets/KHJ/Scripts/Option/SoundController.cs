using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    private Scrollbar masterVolumeSlider;
    private Scrollbar uiVolumeSlider;
    private Scrollbar bgmVolumeSlider;
    private Scrollbar effectVolumeSlider;

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

        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        uiVolumeSlider.onValueChanged.AddListener(UpdateUIVolume);
        bgmVolumeSlider.onValueChanged.AddListener(UpdateBgmVolume);
        effectVolumeSlider.onValueChanged.AddListener(UpdateEffectVolume);
    }

    private void UpdateMasterVolume(float value)
    {
        SoundManager.Instance.SetVolume(SoundType.UI, value);
        SoundManager.Instance.SetVolume(SoundType.BGM, value);
        SoundManager.Instance.SetVolume(SoundType.EFFECT, value);
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
