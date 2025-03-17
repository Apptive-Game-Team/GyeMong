using Creature.Player.Component;
using playerCharacter;
using UnityEngine;

namespace System.Game.Buff.Data
{

    [CreateAssetMenu(fileName = "ShieldBuffData", menuName = "BuffData/Shield", order = 3)]
    public class ShieldBuffData : BuffData
    {
        public float amount;

        public override void ApplyEffect(IBuffable target)
        {
            ((PlayerCharacter)target).curShield = amount;
            BuffEvents.TriggerBuffApplied(this, target); 
        }

        public override void RemoveEffect(IBuffable target)
        {
            PlayerCharacter player = (PlayerCharacter)target;
            if (player.curShield <= amount) player.curShield = 0;  
            else player.curShield -= amount;
            BuffEvents.TriggerBuffExpired(this, target); 
        }
    }
}
