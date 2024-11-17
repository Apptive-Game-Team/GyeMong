using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RuneData
{
    public string name;
    public int id;
    public Sprite runeImage;
    public BuffData runeBuff;
    public float cooldown;
}

[CreateAssetMenu(fileName = "RuneDataList",menuName ="ScriptableObject/RuneDataList")]
public class RuneDataList : ScriptableObject
{
    public List<RuneData> runeDataList;
}
