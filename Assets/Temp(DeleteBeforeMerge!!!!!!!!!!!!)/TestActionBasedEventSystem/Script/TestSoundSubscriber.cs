using System;
using System.Collections;
using System.Collections.Generic;
using Gyemong.SoundSystem;
using UnityEngine;

public class TestSoundSubscriber : MonoBehaviour
{
    [SerializeField] SoundSourceList soundSourceList;
    private AudioSource _audioSource;
    
    private void OnEnable() => TestActor.OnCombatStateChanged += CallSound;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void CallSound(bool isBattle)
    {
        if (isBattle)
        {
            _audioSource.pitch = 0.5f;
            // _audioSource.clip = soundSourceList.GetSoundSourceByName("BattleBGM").clip;
            // _audioSource.Play();
        }
        else
        {
            _audioSource.pitch = 1f;
            // _audioSource.clip = soundSourceList.GetSoundSourceByName("NormalBGM").clip;
            // _audioSource.Play();
        }
    }
}
