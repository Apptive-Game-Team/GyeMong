using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMappingUI : SingletonObject<KeyMappingUI>
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenKeyMappingUI()
    {
        gameObject.SetActive(true);
    }
}
