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
    RUNE_DEFAULT = 100,
    RUNE_BREEZE = 101,
    RUNE_VINE = 102,
    RUNE_FLOWER = 103,
    RUNE_GOLEM = 104,
}

//�ʿ��� ��� - ���� ���׾� ���°� ������ �Ǿ���� (�ӹڻ���)

public enum BuffDisposeMode
{
    NONE = 0,
    TEMPORARY = 1,
    PERMANENT = 2,
}

public enum BuffActiveMode
{
    NONE = 0,
    EVERY_SOME_SECOND = 1,
    ON_ATTACK = 2,
}



[Serializable]
public class BuffData
{
    public BuffType buffType;
    public BuffDisposeMode disposeMode;
    public BuffActiveMode buffActiveMode;
    public float duration;
    public float amount1;
    public float amount2;
}

public interface IBuffSlave
{
    public void AddBuff(BuffObject newBuff);
    public void DeleteBuff(BuffObject newBuff);
}

public interface IBuffMaster
{
    public void AddRuneEvent(BuffObject data, BuffComponent buffComp);
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

public class BuffManager : SingletonObject<BuffManager>, IBuffMaster
{
    [SerializeField] public RuneDataList runeDataList;
    RuneEventListenerCaller _runeEventListenerCaller = new ();
    
    BuffHitman buffHitman;

    private int _nowBuffObjectID = 0;

    public int MakeBuffObjectID()
    {
        int toReturnID = _nowBuffObjectID;
        _nowBuffObjectID++;
        return toReturnID;
    }
    
    private void Start()
    {
        buffHitman = new BuffHitman();
    }
    
    public void AddRuneEvent(BuffObject data, BuffComponent buffComp)
    {
        //listener Add
        // _runeEventListenerCaller.AddRuneEventListener();//Creature...
        // if (!buffComp.HasBuff(data))
        // {
        //     StartCoroutine(AddEverySecondBuff(data, buffComp));
        //     Debug.Log(data.buffType);   
        // }
        
    }
    public void DeleteRuneEvent(BuffObject data, BuffComponent buffComp)
    {
        buffComp.DeleteBuff(data);
    }

    //we need buffhandler.... tick dmg/heal or dispose by time...

    public IEnumerator AddTemporaryBuff(BuffObject data, BuffComponent buffComp)
    {
        return null;
    }

    public IEnumerator AddEverySecondBuff(BuffObject data, BuffComponent buffComp)
    {
        return null;
    }
}
