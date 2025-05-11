using System;
using System.Collections;
using System.Collections.Generic;
using Gyemong.SoundSystem;
using UnityEngine;

public class TestSoundSubscriber2 : MonoBehaviour
{
    [SerializeField] SoundSourceList soundSourceList;
    private AudioSource _audioSource;
    
    private void OnEnable() => TestActor.OnPlayerGrazed += CallSound;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void CallSound()
    {
        _audioSource.Play();
    }
}
