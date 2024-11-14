using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class KeyMapping : MonoBehaviour
{
    protected abstract ActionCode ActionCode { get; }
    protected abstract String InitialCode { get; }
    private TextMeshProUGUI keyText;

    private void Start()
    {
        keyText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        keyText.text = InitialCode;
    }
    
    protected void BindKey(KeyCode newKey)
    {
        InputManager.Instance.SetKey(ActionCode, newKey);
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
        UpdateKeyText(pressedKey);
    }

    private KeyCode GetPressedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key)) return key;
        }
        return KeyCode.None;
    }

    private void UpdateKeyText(KeyCode keyCode)
    {
        keyText.text = keyCode.ToString();
    }
}
