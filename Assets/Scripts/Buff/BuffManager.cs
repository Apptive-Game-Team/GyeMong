using System;
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

[Serializable]
public class BuffData
{
    public BuffType buffType;
    public BuffDisposeMode disposeMode;
    public float duration;
    public float amount1;
    public float amount2;
}

public interface IBuffSlave
{
    public void AddBuff(BuffData newBuff);
    public void DeleteBuff(BuffData newBuff);
}

public interface IBuffMaster
{
    public void AddRuneEvent(BuffData data, BuffComponent buffComp);
}


[Serializable]
public class BuffComponent : IBuffSlave
{
    [SerializeField] List<BuffData> buffList = new List<BuffData>();

    public void AddBuff(BuffData newBuff)
    {
        switch(newBuff.disposeMode)
        {
            case BuffDisposeMode.TEMPORARY:
                buffList.Add(newBuff);
                //listener.AddEvent(DisposeEvent,this,disposeTime);
                BuffManager.Instance.StartCoroutine(BuffManager.Instance.AddTemporaryBuff(newBuff, this));
                break;
            case BuffDisposeMode.PERMANENT:
                buffList.Add(newBuff);
                break;
        }
    }

    public void DeleteBuff(BuffData buff)
    {
        buffList.Remove(buff);
    }
}

public class BuffManager : SingletonObject<BuffManager>, IBuffMaster
{
    [SerializeField] RuneDataList runeDataList;
    public void AddRuneEvent(BuffData data, BuffComponent buffComp)
    {
        Debug.Log(data.buffType);
    }
    public void DeleteRuneEvent(BuffData data, BuffComponent buffComp)
    {
        buffComp.DeleteBuff(data);
    }

    //we need buffhandler.... tick dmg/heal or dispose by time...

    public IEnumerator AddTemporaryBuff(BuffData data, BuffComponent buffComp)
    {
        Debug.Log(data.buffType);
        yield return new WaitForSeconds(data.duration);
        DeleteRuneEvent(data, buffComp);
        Debug.Log(data.duration);
    }
}
