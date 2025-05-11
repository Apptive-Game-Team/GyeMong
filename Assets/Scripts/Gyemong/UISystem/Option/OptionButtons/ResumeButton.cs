using UnityEngine;

namespace Gyemong.UISystem.Option.OptionButtons
{
    public class ResumeButton : MonoBehaviour
    {
        public void OnclickButton()
        {
            OptionUIToggler.Instance.ToggleOption();
        }
    }
}
