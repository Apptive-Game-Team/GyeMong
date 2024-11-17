using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class KeyButtonTexts : SingletonObject<KeyButtonTexts>
{
    private List<Transform> keys = new();
    private List<TextMeshProUGUI> keyTexts = new();

    private void Start()
    {
        for (int i = 0;i < transform.childCount;i++)
        {
            keys.Add(transform.GetChild(i));
        }

        for (int i = 0;i < keys.Count;i++) 
        {
            keyTexts.Add(keys[i].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>());
        }

        UpdateKeyText();
    }


    public void UpdateKeyText()
    {
        Dictionary<ActionCode, KeyCode> keyMappings = InputManager.Instance.GetKeyActions();
        for (int i = 0;i < keyTexts.Count;i++) 
        {
            string keyCodeName = keys[i].name;
            keyTexts[i].text = keyMappings[(ActionCode)Enum.Parse(typeof(ActionCode), keyCodeName)].ToString();
        }
    }
}
