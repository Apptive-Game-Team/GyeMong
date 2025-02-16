using UnityEngine;

public class KeyMappingButton : MonoBehaviour
{
    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = false;
        KeyButtonTexts.Instance.UpdateKeyText();
        KeyMappingUI.Instance.OpenKeyMappingUI();
    }
}
