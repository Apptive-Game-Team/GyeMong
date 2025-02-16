using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControlExitButton : MonoBehaviour
{
    private Image SoundControlImage;
    private SoundData soundData = new();

    private void Start()
    {
        Transform SoundControlImageTransform = transform.parent;
        SoundControlImage = SoundControlImageTransform.GetComponent<Image>();
    }

    public void OnClickButton()
    {
        OptionUI.Instance.isOptionUITop = true;
        SoundControlImage.gameObject.SetActive(false);

        soundData.masterVolume = SoundController.Instance.masterVolumeSlider.value;
        soundData.UIVolume = SoundController.Instance.uiVolumeSlider.value;
        soundData.bgmVolume = SoundController.Instance.bgmVolumeSlider.value;
        soundData.sfxVolume = SoundController.Instance.effectVolumeSlider.value;

        DataManager.Instance.SaveSection<SoundData>(soundData, "SoundData");
    }
}
