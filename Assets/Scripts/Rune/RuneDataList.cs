using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RuneData
{
    public int parentID;
    public string name;
    public int id;
    public string description;
    public Sprite runeImage;
    public BuffData runeBuff;
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
}
