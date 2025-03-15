using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public class RuneData
{
    public int parentID;
    public string name;
    public int id;
    public string description;
    public int cost;
    public Sprite runeImage;
    public List<RuneUpgrade> availableOptions;
    public bool isUnlocked;
}
[Serializable]
public class RuneUpgrade
{
    public string name;
    public int id;
    public int parentRuneID;
    public string description;
    public Sprite upgradeImage;
    public bool isUnlocked;
}

[CreateAssetMenu(fileName = "RuneDataList",menuName ="ScriptableObject/RuneDataList")]
public class RuneDataList : ScriptableObject
{
    public List<RuneData> runeDataList;

    public RuneData GetRuneData(int id)
    {
        RuneData runeData = runeDataList.Find(x => x.id == id);
        if (runeData == null)
        {
            Debug.LogError("There's No Rune that has this id!");
            return null;
        }

        return runeData;
    }

    public RuneUpgrade GetRuneUpgrade(RuneData parentRune,int id)
    {
        RuneUpgrade runeUpgrade = parentRune.availableOptions[id];
        return runeUpgrade;
    }
}
