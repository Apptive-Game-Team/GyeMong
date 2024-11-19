using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyMappingButton : MonoBehaviour
{
    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = false;
        KeyMappingData keyMappingData = DataManager.Instance.LoadSection<KeyMappingData>("KeyMappingData");
        for (int i = 0;i < keyMappingData.keyBindings.Count;i++) 
        {
            KeyMappingEntry entry = keyMappingData.keyBindings[i];

            ActionCode actionCode = (ActionCode)Enum.Parse(typeof(ActionCode), entry.actionCode);
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), entry.keyCode);

            InputManager.Instance.SetKey(actionCode, keyCode);
        }
        KeyButtonTexts.Instance.UpdateKeyText();
        KeyMappingUI.Instance.OpenKeyMappingUI();
    }
}
