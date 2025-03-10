using System;
using System.Collections;
using System.Collections.Generic;
using System.Game.Buff;
using System.Game.Buff.Component;
using Creature.Player.Component;
using playerCharacter;
using UnityEngine;
using UnityEngine.Serialization;

//Something that can receive Buffs.
public interface IBuffable
{
    
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
