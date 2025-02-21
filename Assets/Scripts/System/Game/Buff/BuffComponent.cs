using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffComponent : MonoBehaviour ,IBuffSlave
{
    [SerializeField] List<BuffObject> buffList = new List<BuffObject>();

    public void AddBuff(BuffObject newBuff)
    {
        buffList.Add(newBuff);
        switch(newBuff.buffData.disposeMode)
        {
            case BuffDisposeMode.TEMPORARY:
                BuffManager.Instance.StartCoroutine(BuffManager.Instance.AddTemporaryBuff(newBuff, this));
                break;
            case BuffDisposeMode.PERMANENT:
                BuffManager.Instance.AddRuneEvent(newBuff, this);
                break;
        }
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

