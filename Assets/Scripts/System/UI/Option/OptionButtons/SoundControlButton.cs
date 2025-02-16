using UnityEngine;

public class SoundControlButton : MonoBehaviour
{
    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = false;
        SoundControlUI.Instance.OpenSoundControlImage();
    }
}
