using Gyemong.UISystem.Option.SoundControl;
using UnityEngine;

namespace Gyemong.UISystem.Option.OptionButtons
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
