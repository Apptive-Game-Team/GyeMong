using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControlButton : MonoBehaviour
{
    private Image SoundControlImage;

    private void Start()
    {
        Transform SoundControllImageTransform = transform.Find("SoundControlImage");
        SoundControlImage = SoundControllImageTransform.GetComponent<Image>();
        SoundControlImage.gameObject.SetActive(false);
    }

    public void OnClickButton()
    {
        Option.Instance.OpenOrCloseOption();
        SoundControlImage.gameObject.SetActive(true);
    }
}
