using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SoundObject : MonoBehaviour
{
    private SoundManager soundManager;

    [SerializeField] private string sourceName = null;
    private SoundType soundType;

    private AudioClip clip;
    private AudioSource audioSource;
    private float volume;
    private float masterVolume = 1f;
    public bool IsPlaying {
        get { return audioSource.isPlaying; }
    }

    private void Awake()
    {
        masterVolume = DataManager.Instance.LoadSection<SoundData>("SoundData") == null ? 1f : DataManager.Instance.LoadSection<SoundData>("SoundData").masterVolume;
    }

    private void Start()
    {
        soundManager = SoundManager.Instance;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (sourceName != null && sourceName.Length != 0)
        {
            SetSoundSourceByName(sourceName);
        }
    }

    public void SetLoop(bool isLoop)
    {
        audioSource.loop = isLoop;
    }

    public void SetSoundSourceByName(string soundSourceName)
    {
        SoundSource soundSource = SoundManager.Instance.soundSourceList.GetSoundSourceByName(soundSourceName);
        soundType = soundSource.type;
        volume = SoundManager.Instance.GetVolume(soundSource.type);
        clip = soundSource.clip;
    }

    public void SetSoundSource(SoundSource soundSource)
    {
        soundType = soundSource.type;
        volume = SoundManager.Instance.GetVolume(soundSource.type);
        clip = soundSource.clip;
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public IEnumerator Play()
    {
        yield return new WaitUntil(()=>audioSource != null);
        audioSource.volume = volume * masterVolume;
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitWhile(()=>audioSource.isPlaying);
    }

    public void PlayAsync()
    {
        StartCoroutine(Play());
    }

    public SoundType GetSoundType()
    {
        return soundType;
    }

    public void SetVolume(float volume)
    {
        this.volume = volume;
        UpdateAudioSourceVolume();
    }

    public void SetMasterVolume(float masterVolume)
    {
        this.masterVolume = masterVolume;
        UpdateAudioSourceVolume();
    }

    private void UpdateAudioSourceVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume * masterVolume;
        }
    }
}
