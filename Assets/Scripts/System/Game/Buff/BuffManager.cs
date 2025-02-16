using System;
using System.Collections;
using System.Collections.Generic;
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
    public void AddBuff(BuffData newBuff);
    public void DeleteBuff(BuffData newBuff);
}

public interface IBuffMaster
{
    public void AddRuneEvent(BuffData data, BuffComponent buffComp);
}

public class BuffManager : SingletonObject<BuffManager>, IBuffMaster
{
    [SerializeField] public RuneDataList runeDataList;
    BuffHitman buffHitman;

    private void Start()
    {
        buffHitman = new BuffHitman();
    }
    public void AddRuneEvent(BuffData data, BuffComponent buffComp)
    {
        StartCoroutine(AddEverySecondBuff(data, buffComp));
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

    public IEnumerator AddEverySecondBuff(BuffData data, BuffComponent buffComp)
    {
        while (true) 
        {
            yield return new WaitForSeconds(data.duration);
            switch (data.buffType)
            {
                //Character Health not yet Implement...
                case BuffType.RUNE_BREEZE:
                    buffHitman.ActiveRune_Breeze(data);
                    break;
                case BuffType.RUNE_VINE:
                    buffHitman.ActiveRune_Vine(data);
                    break;
            }
        }
    }
}
