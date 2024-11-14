using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMappingButton : MonoBehaviour
{
    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = false;
        KeyMappingUI.Instance.OpenKeyMappingUI();
    }
}
