using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControlButton : MonoBehaviour
{
    public void OnClickButton()
    {
        Option.Instance.OpenOrCloseOption();
        SoundControlImage.Instance.OpenSoundControlImage();
    }
}
