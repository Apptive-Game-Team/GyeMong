using GyeMong.DataSystem;
using GyeMong.SoundSystem;
using UnityEngine;
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
        public Scrollbar friendlyVolumeSlider;
        public Scrollbar enemyVolumeSlider;

        public void InitialSetting()
        {
            masterVolumeSlider = transform.Find("MasterVolumeSlider").GetComponent<Scrollbar>();
            uiVolumeSlider = transform.Find("UIVolumeSlider").GetComponent<Scrollbar>();
            bgmVolumeSlider = transform.Find("BgmVolumeSlider").GetComponent<Scrollbar>();
            effectVolumeSlider = transform.Find("EffectVolumeSlider").GetComponent<Scrollbar>();
            friendlyVolumeSlider = transform.Find("FriendlyVolumeSlider").GetComponent<Scrollbar>();
            enemyVolumeSlider = transform.Find("EnemyVolumeSlider").GetComponent<Scrollbar>();

            SoundData soundData = DataManager.Instance.LoadSection<SoundData>("SoundData");
            masterVolumeSlider.value = soundData.masterVolume;
            uiVolumeSlider.value = soundData.UIVolume;
            bgmVolumeSlider.value = soundData.bgmVolume;
            effectVolumeSlider.value = soundData.sfxVolume;
            friendlyVolumeSlider.value = soundData.friendlyVolume;
            enemyVolumeSlider.value = soundData.enemyVolume;

            SoundManager.Instance.SetMasterVolume(soundData.masterVolume);
            SoundManager.Instance.SetVolume(SoundType.UI, soundData.UIVolume);
            SoundManager.Instance.SetVolume(SoundType.BGM, soundData.bgmVolume);
            SoundManager.Instance.SetVolume(SoundType.EFFECT, soundData.sfxVolume);
            SoundManager.Instance.SetVolume(SoundType.FRIENDLY, soundData.friendlyVolume);
            SoundManager.Instance.SetVolume(SoundType.ENEMY, soundData.enemyVolume);

            masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
            uiVolumeSlider.onValueChanged.AddListener(UpdateUIVolume);
            bgmVolumeSlider.onValueChanged.AddListener(UpdateBgmVolume);
            effectVolumeSlider.onValueChanged.AddListener(UpdateEffectVolume);
            friendlyVolumeSlider.onValueChanged.AddListener(UpdateFriendlyVolume);
            enemyVolumeSlider.onValueChanged.AddListener(UpdateEnemyVolume);
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
        
        private void UpdateFriendlyVolume(float value)
        {
            SoundManager.Instance.SetVolume(SoundType.FRIENDLY, value);
        }
        
        private void UpdateEnemyVolume(float value)
        {
            SoundManager.Instance.SetVolume(SoundType.ENEMY, value);
        }
    }
}
