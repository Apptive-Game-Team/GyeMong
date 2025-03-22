using System.UI.Option.KeyMapping;
using UnityEngine;

namespace System.UI.Option.OptionButtons
{
    public class KeyMappingButton : MonoBehaviour
    {
        public void OnClickButton()
        {
            OptionUI.Instance.isOptionUITop = false;
            KeyButtonTexts.Instance.UpdateKeyText();
            KeyMappingUI.Instance.OpenKeyMappingUI();
        }
    }
}
