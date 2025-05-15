using UnityEngine;

namespace GyeMong.UISystem.Option.OptionButtons
{
    public class ResumeButton : MonoBehaviour
    {
        public void OnclickButton()
        {
            OptionUIToggler.Instance.ToggleOption();
        }
    }
}
