using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundControlImage : SingletonObject<SoundControlImage>
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenSoundControlImage()
    {
        gameObject.SetActive(true);
    }
}
