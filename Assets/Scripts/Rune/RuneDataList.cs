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
}

[CreateAssetMenu(fileName = "RuneDataList",menuName ="ScriptableObject/RuneDataList")]
public class RuneDataList : ScriptableObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
