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

        soundData.masterVolume = OptionUI.Instance.GetSoundController.masterVolumeSlider.value;
        soundData.UIVolume = OptionUI.Instance.GetSoundController.uiVolumeSlider.value;
        soundData.bgmVolume = OptionUI.Instance.GetSoundController.bgmVolumeSlider.value;
        soundData.sfxVolume = OptionUI.Instance.GetSoundController.effectVolumeSlider.value;

        DataManager.Instance.SaveSection<SoundData>(soundData, "SoundData");
    }
}
