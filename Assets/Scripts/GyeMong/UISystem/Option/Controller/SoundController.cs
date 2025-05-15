using GyeMong.DataSystem;
using GyeMong.SoundSystem;
using UnityEngine.UI;
using Util;

namespace GyeMong.UISystem.Option.Controller
{
    public class SoundController : SingletonObject<SoundController>
    {
        public Scrollbar masterVolumeSlider;
        public Scrollbar uiVolumeSlider;
        public Scrollbar bgmVolumeSlider;
        public Scrollbar effectVolumeSlider;

        private void Start()
        {
            InitialSetting();
        }

        private void InitialSetting()
        {
            masterVolumeSlider = transform.Find("MasterVolumeSlider").GetComponent<Scrollbar>();
            uiVolumeSlider = transform.Find("UIVolumeSlider").GetComponent<Scrollbar>();
            bgmVolumeSlider = transform.Find("BgmVolumeSlider").GetComponent<Scrollbar>();
            effectVolumeSlider = transform.Find("EffectVolumeSlider").GetComponent<Scrollbar>();

            SoundData soundData = DataManager.Instance.LoadSection<SoundData>("SoundData");
            masterVolumeSlider.value = soundData.masterVolume;
            uiVolumeSlider.value = soundData.UIVolume;
            bgmVolumeSlider.value = soundData.bgmVolume;
            effectVolumeSlider.value = soundData.sfxVolume;

            SoundManager.Instance.SetMasterVolume(soundData.masterVolume);
            SoundManager.Instance.SetVolume(SoundType.UI, soundData.UIVolume);
            SoundManager.Instance.SetVolume(SoundType.BGM, soundData.bgmVolume);
            SoundManager.Instance.SetVolume(SoundType.EFFECT, soundData.sfxVolume);

            masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
            uiVolumeSlider.onValueChanged.AddListener(UpdateUIVolume);
            bgmVolumeSlider.onValueChanged.AddListener(UpdateBgmVolume);
            effectVolumeSlider.onValueChanged.AddListener(UpdateEffectVolume);
        }

        private void UpdateMasterVolume(float value)
        {
            SoundManager.Instance.SetMasterVolume(value);
        }

        private void UpdateUIVolume(float value)
        {
            SoundManager.Instance.SetVolume(SoundType.UI, value);
        }

        private void UpdateBgmVolume(float value)
        {
            SoundManager.Instance.SetVolume(SoundType.BGM, value);
        }

        private void UpdateEffectVolume(float value)
        {
            SoundManager.Instance.SetVolume(SoundType.EFFECT, value);
        }
    
    }
}
