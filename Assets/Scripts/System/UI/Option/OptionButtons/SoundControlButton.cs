using System.UI.Option.SoundControl;
using UnityEngine;

namespace System.UI.Option.OptionButtons
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
