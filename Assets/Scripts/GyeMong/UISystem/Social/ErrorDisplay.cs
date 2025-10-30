using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorDisplay : MonoBehaviour
{
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMP_Text errorText;
    
    public void ShowError(string message)
    {
        errorText.text = message;
        errorPanel.SetActive(true);
        Invoke(nameof(HideError), 2f);
    }
    
    private void HideError()
    {
        errorPanel.SetActive(false);
    }
    
    
}
