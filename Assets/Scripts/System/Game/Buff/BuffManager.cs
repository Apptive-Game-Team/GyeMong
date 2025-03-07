using System;
using System.Collections;
using System.Collections.Generic;
using System.Game.Buff.Component;
using UnityEngine;

public enum BuffType
{
    DEFAULT = 0,
    BUFF_SNARE = 1,
    BUFF_DOT_DAMAGE = 2,
    BUFF_DOT_HEAL = 4,
}

[Serializable]
public class BuffData
{
    public BuffType buffType;
    public float duration;
    public float amount1;
    public float amount2;
}

public class BuffObject //In-Game Buff Object... It has Data and In-Game Interact with other In-Game Object.
{
    public int ID;
    public BuffData buffData;

    public BuffObject(BuffData data)
    {
        ID = BuffManager.Instance.MakeBuffObjectID();
        buffData = data;
    }
}

public interface IBuffTriggerable
{
    
}

public class BuffManager : SingletonObject<BuffManager>
    {
        [SerializeField] public RuneDataList runeDataList;

        private int _nowBuffObjectID = 0;

        public int MakeBuffObjectID()
        {
            int toReturnID = _nowBuffObjectID;
            _nowBuffObjectID++;
            return toReturnID;
        }

        public IEnumerator AddTemporaryBuff(BuffObject data, BuffComponent buffComp)
        {
            return null;
        }
        
    }
