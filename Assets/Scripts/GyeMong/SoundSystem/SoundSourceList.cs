using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.SoundSystem
{
    [Serializable]
    public struct SoundSource {
        public string name;
        public SoundType type;
        public AudioClip clip;
    }

    [CreateAssetMenu(fileName = "SoundSourceList", menuName = "ScriptableObject/New SoundSourceList")]
    public class SoundSourceList : ScriptableObject
    {
        [SerializeField]
        public List<SoundSource> soundSources = new List<SoundSource>();

        public SoundSource GetSoundSourceByName(string name)
        {
        
            foreach (SoundSource source in soundSources)
            {
                if (source.name.Equals(name))
                    return source;
            }

            throw new Exception("SoundSource is not found by name: " + name);
        }

        public List<SoundSource> GetSoundSourcesByNameComtains(string subString)
        {
            List<SoundSource> result = new List<SoundSource>();
            foreach (SoundSource source in soundSources)
            {
                if (source.name.Contains(subString))
                    result.Add(source);
            }
            return result;
        } 
    }
}