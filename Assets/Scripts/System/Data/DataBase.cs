using System;
using System.Collections.Generic;
using System.Game.Quest.Quests;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public bool isFirst = true;
    public string sceneName = "SpringBeachScene";
    public Vector3 playerPosition = new(4.93f, -1.72f, 0);
    public Vector2 playerDirection = new(1, 0);
}

[System.Serializable]
public class RuneDatas
{
    public List<RuneData> AcquiredRuneDatas = new();
    public List<RuneData> EquippedRuneDatas = new();
}

[Serializable]
public class QuestData
{
    public List<SerializableQuestInfo> quests = new();
}

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