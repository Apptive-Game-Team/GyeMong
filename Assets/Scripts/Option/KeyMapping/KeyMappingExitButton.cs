using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyMappingExitButton : MonoBehaviour
{
    private Image keyMappingUI;
    private KeyMappingData keyMappingData = new();

    private void Start()
    {
        Transform keyMappingUITransform = transform.parent;
        keyMappingUI = keyMappingUITransform.GetComponent<Image>();
    }

    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = true;
        keyMappingUI.gameObject.SetActive(false);

        keyMappingData.keyBindings.Clear();
        foreach (var pair in InputManager.Instance.GetKeyActions())
        {
            keyMappingData.keyBindings.Add(new KeyMappingEntry(pair.Key, pair.Value));
        }

        DataManager dataManager = new();
        dataManager.SaveSection(keyMappingData, "KeyMappingData");
    }

}
