using GyeMong.DataSystem;
using GyeMong.UISystem.Option.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Option.SoundControl
{
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
            soundData.friendlyVolume = SoundController.Instance.friendlyVolumeSlider.value;
            soundData.enemyVolume = SoundController.Instance.enemyVolumeSlider.value;

            DataManager.Instance.SaveSection<SoundData>(soundData, "SoundData");
        }
    }
}
