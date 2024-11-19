using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public float masterVolume = 1f;
    public float UIVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
}

[System.Serializable]
public class KeyMappingData
{
    public List<KeyMappingEntry> keyBindings = new();
}

[System.Serializable]
public class KeyMappingEntry
{
    public string actionCode;
    public string keyCode;

    public KeyMappingEntry(ActionCode actionCode, KeyCode keyCode)
    {
        this.actionCode = actionCode.ToString();
        this.keyCode = keyCode.ToString();
    }
}