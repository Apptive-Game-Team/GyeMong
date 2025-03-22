using UnityEngine;

namespace System.Game.Buff.Data
{
    [CreateAssetMenu(fileName = "PlainBuffData", menuName = "BuffData/PlainBuff", order = 0)]
    public class PlainBuffData : BuffData
    {
        public override void ApplyEffect(IBuffable target)
        {
            BuffEvents.TriggerBuffApplied(this, target);
        }

        public override void RemoveEffect(IBuffable target)
        {
            BuffEvents.TriggerBuffExpired(this, target);
        }
    }
}
