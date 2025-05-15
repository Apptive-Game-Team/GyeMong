using GyeMong.UISystem.Option.SoundControl;
using UnityEngine;

namespace GyeMong.UISystem.Option.OptionButtons
{
    public class SoundControlButton : MonoBehaviour
    {
        public void OnClickButton()
        {
            OptionUI.Instance.isOptionUITop = false;
            SoundControlUI.Instance.OpenSoundControlImage();
        }
    }
}
