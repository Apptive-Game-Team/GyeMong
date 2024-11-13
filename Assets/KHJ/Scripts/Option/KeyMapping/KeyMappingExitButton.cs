using UnityEngine;
using UnityEngine.UI;

public class KeyMappingExitButton : MonoBehaviour
{
    private Image keyMappingUI;

    private void Start()
    {
        Transform keyMappingUITransform = transform.parent;
        keyMappingUI = keyMappingUITransform.GetComponent<Image>();
    }

    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = true;
        keyMappingUI.gameObject.SetActive(false);
    }
}
