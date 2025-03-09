using System;
using System.Collections;
using System.Collections.Generic;
using System.Game.Buff.Component;
using Creature.Player.Component;
using playerCharacter;
using UnityEngine;
using UnityEngine.Serialization;

public interface IBuffable
{
    
}

public enum BuffType
{
    DEFAULT = 0,
    BUFF_SNARE = 1,
    BUFF_DOT_DAMAGE = 2,
    BUFF_DOT_HEAL = 4,
}

public static class BuffEvents
{
    public static event Action<BuffData, IBuffable> OnBuffApplied;
    public static event Action<BuffData, IBuffable> OnBuffExpired;

    public static void TriggerBuffApplied(BuffData buff, IBuffable target)
    {
        OnBuffApplied?.Invoke(buff, target);
    }

    public static void TriggerBuffExpired(BuffData buff, IBuffable target)
    {
        OnBuffExpired?.Invoke(buff, target);
    }
}

public class Buff
{
    public BuffData buffData;
    public float remainingTime;
    public int curStack;

    public IBuffable target;
    
    public bool CanStack => curStack > buffData.maxStack;
    
    public Buff(BuffData data)
    {
        buffData = data;
        remainingTime = data.duration;
    }

    public void UpdateBuff(float deltaTime)
    {
        if (!buffData.isPermanent)
        {
            remainingTime -= deltaTime;
        }
    }

    public bool IsExpired() => !buffData.isPermanent && remainingTime <= 0;
}


public abstract class BuffData : ScriptableObject
{
    public float duration;
    public bool isPermanent;
    public int maxStack;
    
    public abstract void ApplyEffect(IBuffable target);
    public abstract void RemoveEffect(IBuffable target);
}

[CreateAssetMenu(fileName = "StatBuffData", menuName = "BuffData/StatBuff")]
public class StatBuffData : BuffData
{
    public StatType StatType;
    public StatValueType StatValueType;
    public float value;

    public override void ApplyEffect(IBuffable target)
    {
        StatComponent statComponent = ((PlayerCharacter)target).GetComponent<StatComponent>();
        statComponent?.SetStatValue(StatType, StatValueType, value);
        BuffEvents.TriggerBuffApplied(this, target); 
    }

    public override void RemoveEffect(IBuffable target)
    {
        StatComponent statComponent = ((PlayerCharacter)target).GetComponent<StatComponent>();
        statComponent?.SetStatValue(StatType, StatValueType, -value);
        BuffEvents.TriggerBuffApplied(this, target); 
    }
}

[CreateAssetMenu(fileName = "DOTBuffData", menuName = "BuffData/DOT")]
public class DotBuffData : BuffData
{
    public float damagePerTick;
    public float tickInterval;

    public override void ApplyEffect(IBuffable target)
    {
        BuffManager.Instance.ApplyDotEffect(target,this);
        BuffEvents.TriggerBuffApplied(this, target);
    }

    public override void RemoveEffect(IBuffable target)
    {
        BuffEvents.TriggerBuffExpired(this, target);
    }
}

public class BuffManager : SingletonObject<BuffManager>
{
    public BuffData breezeRuneData;
    
    List<Buff> activeBuffList = new List<Buff>();
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < activeBuffList.Count; i++)
        {
            Buff buff = activeBuffList[i];
            buff.UpdateBuff(deltaTime);

            if (buff.IsExpired())
            {
                RemoveBuff(buff);
            }
        }
    }
    
    public void ApplyBuff(IBuffable target, BuffData buffData)
    {
        Buff newBuff = new Buff(buffData);
        newBuff.target = target;
        activeBuffList.Add(newBuff);
        buffData.ApplyEffect(target);
    }

    private void RemoveBuff(Buff buff)
    {
        buff.buffData.RemoveEffect(buff.target);
        activeBuffList.Remove(buff);
    }

    public void ApplyDotEffect(IBuffable target, DotBuffData dotBuff)
    {
        StartCoroutine(ApplyDotCoroutine(target, dotBuff));
    }

    private IEnumerator ApplyDotCoroutine(IBuffable target, DotBuffData dotBuff)
    {
        float elapsedTime = 0;
        while (dotBuff.isPermanent || elapsedTime < dotBuff.duration)
        {
            BuffEvents.TriggerBuffApplied(dotBuff,target);
            PlayerCharacter.Instance.Heal(-dotBuff.damagePerTick);
            yield return new WaitForSeconds(dotBuff.tickInterval);
            elapsedTime += dotBuff.tickInterval;
        }
        BuffEvents.TriggerBuffExpired(dotBuff, target);
    }
}
