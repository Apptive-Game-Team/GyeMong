using System.Collections.Generic;
using System.Game.Buff.Data;
using UnityEngine;

namespace System.Game.Buff
{
    [Serializable]
    public class BuffComponent : MonoBehaviour
    {
        List<BuffData> buffList = new List<BuffData>();

        public void AddBuff(BuffData newBuff)
        {
            buffList.Add(newBuff);
            BuffManager.Instance.ApplyBuff(gameObject.GetComponent<IBuffable>(), newBuff);
        }

        public void DeleteBuff(BuffData buff)
        {
            buffList.Remove(buff);
        }

        public bool HasBuff(BuffData buff)
        {
            return buffList.Contains(buff);
        }
    }
}

