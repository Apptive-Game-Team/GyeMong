using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyMapping : MonoBehaviour
{
    [SerializeField] private ActionCode actionCode;

    private void BindKey(KeyCode newKey)
    {
        CheckDuplication(newKey);
        InputManager.Instance.SetKey(actionCode, newKey);
    }
    
    public void OnClickButton()
    {
        StartCoroutine(WaitForKeyInput());
    }
    
    private IEnumerator WaitForKeyInput()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        
        KeyCode pressedKey = GetPressedKey();
        BindKey(pressedKey);
        KeyButtonTexts.Instance.UpdateKeyText();
    }

    private KeyCode GetPressedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key)) return key;
        }
        return KeyCode.None;
    }
    
    private void CheckDuplication(KeyCode keyCode)
    {
        Dictionary<ActionCode, KeyCode> keyMappings = InputManager.Instance.GetKeyActions();
        List<ActionCode> keysToCheck = new(keyMappings.Keys);

        foreach (ActionCode actionCode in keysToCheck)
        {
            if (keyMappings[actionCode] == keyCode)
            {
                InputManager.Instance.SetKey(actionCode, KeyCode.None);
            }
        }
    }
}
