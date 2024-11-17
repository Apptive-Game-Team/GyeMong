using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
    private List<SoundSource> soundSources = new List<SoundSource>();

    public SoundSource GetSoundSourceByName(string name)
    {
        
        foreach (SoundSource source in soundSources)
        {
            if (source.name.Equals(name))
                return source;
        }

        throw new NotFoundException("SoundSource is not found");
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
