using Creature.Player.Component;
using playerCharacter;
using UnityEngine;

namespace System.Game.Buff
{
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
}
