using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMappingUI : MonoBehaviour
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
