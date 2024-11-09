using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SoundObject : MonoBehaviour
{
    private SoundManager soundManager;

    [SerializeField] private string sourceName = null;

    private AudioClip clip;
    private AudioSource audioSource;
    private float volume;


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

        if (sourceName != null)
        {
            SetSoundSourceByName(sourceName);
        }
    }

    public void SetSoundSourceByName(string soundSourceName)
    {
        SoundSource soundSource = soundManager.soundSourceList.GetSoundSourceByName(soundSourceName);
        volume = soundManager.GetVolume(soundSource.type);
        clip = soundSource.clip;
    }

    public IEnumerator Play()
    {
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitWhile(()=>audioSource.isPlaying);
    }
}
