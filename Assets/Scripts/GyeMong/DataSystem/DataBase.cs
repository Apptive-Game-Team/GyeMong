using System.Collections.Generic;
using GyeMong.InputSystem;
using UnityEngine;

namespace GyeMong.DataSystem
{
    // [System.Serializable]
    // public class PlayerData
    // {
    //     public bool isFirst = true;
    //     public string sceneName = "SpringBeach";
    //     public Vector3 playerPosition = new(4.93f, -1.72f, 0);
    //     public Vector2 playerDirection = new(1, 0);
    // }
    
    [System.Serializable]
    public class SoundData
    {
        public float masterVolume = 0.5f;
        public float UIVolume = 0.5f;
        public float bgmVolume = 0.5f;
        public float sfxVolume = 0.5f;
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
}