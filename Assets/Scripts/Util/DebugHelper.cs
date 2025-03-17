using System.Game.Buff;
using System.Game.Buff.Data;
using Creature.Player;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    void Start()
    {
        BuffEvents.OnBuffApplied += CallApplyMessage;
        BuffEvents.OnBuffExpired += CallExpireMessage;
        PlayerEvent.OnTakeDamage += CallDamageMessage;
    }

    void CallApplyMessage(BuffData buff, IBuffable buffable)
    {
        Debug.Log($"{buffable} Apply Buff {buff}");
    }
    void CallExpireMessage(BuffData buff, IBuffable buffable)
    {
        Debug.Log($"{buffable} Expire Buff {buff}");
    }

    void CallDamageMessage(float amount)
    {
        Debug.Log($"Player Recieved {amount} Damage!");
    }
}
