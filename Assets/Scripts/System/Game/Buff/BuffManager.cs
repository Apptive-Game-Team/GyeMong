using System.Collections;
using System.Collections.Generic;
using System.Game.Buff.Data;
using playerCharacter;
using UnityEngine;

namespace System.Game.Buff
{
    //Something that can receive Buffs.
    public interface IBuffable
    {
    
    }

    public class BuffManager : SingletonObject<BuffManager>
    {
        public BuffData breezeRuneData;
        public BuffData stoneArmorRuneData;
    
        List<Data.Buff> _activeBuffList = new();
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < _activeBuffList.Count; i++)
            {
                Data.Buff buff = _activeBuffList[i];
                buff.UpdateBuff(deltaTime);
                
                if (buff.IsExpired())
                {
                    RemoveBuff(buff);
                }
            }
        }
    
        public void ApplyBuff(IBuffable target, BuffData buffData)
        {
            Data.Buff newBuff = new Data.Buff(buffData);
            newBuff.target = target;
            _activeBuffList.Add(newBuff);
            buffData.ApplyEffect(target);
        }

        private void RemoveBuff(Data.Buff buff)
        {
            buff.buffData.RemoveEffect(buff.target);
            _activeBuffList.Remove(buff);
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
}