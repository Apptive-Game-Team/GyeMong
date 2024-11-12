using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControlExitButton : MonoBehaviour
{
    private Image SoundControlImage;

    private void Start()
    {
        Transform SoundControlImageTransform = transform.parent;
        SoundControlImage = SoundControlImageTransform.GetComponent<Image>();
    }

    public void OnClickButton()
    {
        SoundControlImage.gameObject.SetActive(false);
        OptionUI.Instance.OpenOrCloseOption();
    }
}
