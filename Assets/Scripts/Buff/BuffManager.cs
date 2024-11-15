using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    DEFAULT = 0,
    SNARE = 1,
    DOTDAMAGE = 2,
    DOTHEAL = 3,
    RUNE_DEFAULT = 100,
    RUNE_BREEZE = 101,
    RUNE_VINE = 102,
    RUNE_FLOWER = 103,
    RUNE_GOLEM = 104,
}

public enum BuffDisposeMode
{
    NONE = 0,
    TEMPORARY = 1,
    PERMANENT = 2,
}

public class BuffData
{
    public BuffType buffType;
    public BuffDisposeMode disposeMode;
    public float duration;
    public float disposeTime;
}


public class BuffComponent
{
    List<BuffData> buffList;

    public void AddBuff(BuffData newBuff)
    {
        switch(newBuff.disposeMode)
        {
            case BuffDisposeMode.TEMPORARY:
                buffList.Add(newBuff);
                newBuff.disposeTime = Time.time + newBuff.duration;
                break;
            case BuffDisposeMode.PERMANENT:
                buffList.Add(newBuff);
                break;
        }
    }
}

public class BuffManager : MonoBehaviour
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
