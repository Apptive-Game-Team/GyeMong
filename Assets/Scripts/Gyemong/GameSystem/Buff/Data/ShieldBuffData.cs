using Gyemong.GameSystem.Creature.Player;
using UnityEngine;

namespace Gyemong.GameSystem.Buff.Data
{

    [CreateAssetMenu(fileName = "ShieldBuffData", menuName = "BuffData/Shield", order = 3)]
    public class ShieldBuffData : BuffData
    {
        public float amount;

        public override void ApplyEffect(IBuffable target)
        {
            PlayerCharacter player = (PlayerCharacter)target;
            player.curShield = amount;
            player.changeListenerCaller.CallShieldChangeListeners(amount);
            BuffEvents.TriggerBuffApplied(this, target); 
        }

        public override void RemoveEffect(IBuffable target)
        {
            PlayerCharacter player = (PlayerCharacter)target;
            if (player.curShield <= amount) player.curShield = 0;  
            else player.curShield -= amount;
            player.changeListenerCaller.CallShieldChangeListeners(player.curShield);
            BuffEvents.TriggerBuffExpired(this, target); 
        }
    }
}
