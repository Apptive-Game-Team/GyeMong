using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.SoundSystem
{
    [CreateAssetMenu(fileName = "SoundSourceList", menuName = "ScriptableObject/New SequentialSoundSourceList")]
    public class SequentialSoundSourceList : ScriptableObject
    {
        [SerializeField]
        private List<AudioClip> audioClips = new List<AudioClip>();
        [SerializeField]
        private SoundSource template;

        private bool isInit = false;
        private List<SoundSource> soundSources = null;

        public List<SoundSource> GetSoundSources()
        {
        
            if (!isInit)
            {
                Initialize();
            }
            return soundSources;
        }

        private void Initialize()
        {
            soundSources = new List<SoundSource>();
            int id = 0;
            foreach (AudioClip audioClip in audioClips)
            {
                SoundSource soundSource = new SoundSource();
                soundSource.name = template.name + id;
                soundSource.type = template.type;
                soundSource.clip = audioClip;
                soundSources.Add(soundSource);
                id++;
            }
            isInit = true;
        }

        public SoundSource GetSoundSource(int index)
        {
            if (!isInit)
            {
                Initialize();
            }
            return soundSources[index];
        }

        public int GetLength()
        {
            if (!isInit)
            {
                Initialize();
            }
            return soundSources.Count;
        }
    }
}
