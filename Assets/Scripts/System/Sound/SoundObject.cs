using System.Collections;
using System.Data;
using UnityEngine;

namespace System.Sound
{
    [Serializable]
    public class SoundObject : MonoBehaviour
    {
        private SoundManager _soundManager;

        [SerializeField] private string sourceName = null;
        private SoundType _soundType;

        private AudioClip _clip;
        private AudioSource _audioSource;
        private float _volume;
        private float _masterVolume = 1f;

        private bool _isInitialized = false;
        public bool IsPlaying {
            get { return _audioSource.isPlaying; }
        }

        private void Awake()
        {
            _masterVolume = DataManager.Instance.LoadSection<SoundData>("SoundData") == null ? 1f : DataManager.Instance.LoadSection<SoundData>("SoundData").masterVolume;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            _soundManager = SoundManager.Instance;
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;

            if (sourceName != null && sourceName.Length != 0)
            {
                SetSoundSourceByName(sourceName);
            }

            _isInitialized = true;
        }

        public void SetLoop(bool isLoop)
        {
            _audioSource.loop = isLoop;
        }

        public void SetSoundSourceByName(string soundSourceName)
        {
            SoundSource soundSource = SoundManager.Instance.soundSourceList.GetSoundSourceByName(soundSourceName);
            _soundType = soundSource.type;
            _volume = SoundManager.Instance.GetVolume(soundSource.type);
            _clip = soundSource.clip;
        }

        public void SetSoundSource(SoundSource soundSource)
        {
            _soundType = soundSource.type;
            _volume = SoundManager.Instance.GetVolume(soundSource.type);
            _clip = soundSource.clip;
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public IEnumerator Play()
        {
            yield return new WaitUntil(()=>_audioSource != null);
            _audioSource.volume = _volume * _masterVolume;
            _audioSource.clip = _clip;
            _audioSource.Play();
            yield return new WaitWhile(()=>_audioSource.isPlaying);
        }

        public void PlayAsync()
        {
            StartCoroutine(Play());
        }

        public SoundType GetSoundType()
        {
            return _soundType;
        }

        public void SetVolume(float volume)
        {
            this._volume = volume;
            UpdateAudioSourceVolume();
        }

        public void SetMasterVolume(float masterVolume)
        {
            this._masterVolume = masterVolume;
            UpdateAudioSourceVolume();
        }

        private void UpdateAudioSourceVolume()
        {
            if (_audioSource != null)
            {
                _audioSource.volume = _volume * _masterVolume;
            }
        }
    }
}
