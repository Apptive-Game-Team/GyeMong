using Creature.Player.Component;
using playerCharacter;
using UnityEngine;

namespace System.Game.Buff
{

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
