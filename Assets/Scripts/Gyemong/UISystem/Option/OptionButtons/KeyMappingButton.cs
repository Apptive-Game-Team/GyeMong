using Gyemong.UISystem.Option.KeyMapping;
using UnityEngine;

namespace Gyemong.UISystem.Option.OptionButtons
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
