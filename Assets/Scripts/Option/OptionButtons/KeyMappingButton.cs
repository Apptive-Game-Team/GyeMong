using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyMappingButton : MonoBehaviour
{
    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = false;
        KeyButtonTexts.Instance.UpdateKeyText();
        KeyMappingUI.Instance.OpenKeyMappingUI();
    }
}
