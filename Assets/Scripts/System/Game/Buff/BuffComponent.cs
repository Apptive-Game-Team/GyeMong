using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffComponent
{
    List<BuffObject> buffList = new List<BuffObject>();

    public void AddBuff(BuffObject newBuff)
    {
        buffList.Add(newBuff);
        BuffManager.Instance.StartCoroutine(BuffManager.Instance.AddTemporaryBuff(newBuff, this));
    }

    public void DeleteBuff(BuffObject buff)
    {
        buffList.Remove(buff);
    }

    public bool HasBuff(BuffData buff)
    {
        return buffList.Exists(x => x.buffData.buffType.Equals(buff.buffType));
    }
}

