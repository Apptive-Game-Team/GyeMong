using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SoundType
{
    UI,
    EFFECT,
    FRIENDLY,
    ENEMY,
    BGM
}

public class SoundManager : SingletonObject<SoundManager>
{
    private const float DEFAULT_VOLUME = 1;

    private SoundObject bgmSoundObject;

    public SoundSourceList soundSourceList;
    private Dictionary<SoundType, float> volumes = new Dictionary<SoundType, float>();
    private SoundObject[] soundObjects;
    
    private void InitializeVolumes()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType))){
            volumes[type] = DEFAULT_VOLUME;
        }
    }

    public SoundObject GetBgmObject()
    {
        if (bgmSoundObject == null)
        {
            bgmSoundObject = gameObject.AddComponent<SoundObject>();
        }
        
        return bgmSoundObject;
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeVolumes();
        LoadSoundSettings();
    }

    private void Start()
    {
        soundObjects = FindObjectsOfType<SoundObject>();
    }

    public void SetVolume(SoundType type, float volume)
    {
        volumes[type] = volume;

        foreach (SoundObject soundObject in soundObjects)
        {
            if (soundObject.GetSoundType() == type)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetMasterVolume(float masterVolume)
    {
        foreach (SoundObject soundObject in soundObjects)
        {
            soundObject.SetMasterVolume(masterVolume);
        } 
    }

    public float GetVolume(SoundType type)
    {
        return volumes[type];
    }

    private void LoadSoundSettings()
    {
        SoundData soundData = DataManager.Instance.LoadSection<SoundData>("SoundData");
        volumes[SoundType.UI] = soundData.UIVolume;
        volumes[SoundType.BGM] = soundData.bgmVolume;
        volumes[SoundType.EFFECT] = soundData.sfxVolume;
    }
}
