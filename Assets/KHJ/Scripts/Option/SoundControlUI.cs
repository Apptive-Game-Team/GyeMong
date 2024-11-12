using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundControlUI : SingletonObject<SoundControlUI>
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
