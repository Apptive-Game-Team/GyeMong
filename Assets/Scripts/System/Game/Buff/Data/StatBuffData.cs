using Creature.Player.Component;
using playerCharacter;
using UnityEngine;

namespace System.Game.Buff.Data
{


[CreateAssetMenu(fileName = "StatBuffData", menuName = "BuffData/StatBuff", order = 2)]
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

}
