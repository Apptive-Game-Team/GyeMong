using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffComponent : MonoBehaviour ,IBuffSlave
{
    [SerializeField] List<BuffData> buffList = new List<BuffData>();

    public void AddBuff(BuffData newBuff)
    {
        switch(newBuff.disposeMode)
        {
            case BuffDisposeMode.TEMPORARY:
                buffList.Add(newBuff);
                BuffManager.Instance.StartCoroutine(BuffManager.Instance.AddTemporaryBuff(newBuff, this));
                break;
            case BuffDisposeMode.PERMANENT:
                buffList.Add(newBuff);
                BuffManager.Instance.AddRuneEvent(newBuff, this);
                break;
        }
    }

    public void DeleteBuff(BuffData buff)
    {
        buffList.Remove(buff);
    }
    
    
}

