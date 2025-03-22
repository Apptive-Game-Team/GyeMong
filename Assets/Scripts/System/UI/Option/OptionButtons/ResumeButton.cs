using UnityEngine;

namespace System.UI.Option.OptionButtons
{
    public class ResumeButton : MonoBehaviour
    {
        public void OnclickButton()
        {
            OptionUIToggler.Instance.ToggleOption();
        }
    }
}
