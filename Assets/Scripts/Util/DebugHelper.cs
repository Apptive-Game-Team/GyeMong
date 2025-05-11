using Gyemong.GameSystem.Buff;
using Gyemong.GameSystem.Buff.Data;
using Gyemong.GameSystem.Creature.Player;
using Gyemong.StatusSystem.Gold;
using UnityEngine;

namespace Util
{
    public class DebugHelper : MonoBehaviour
    {
        void Start()
        {
            BuffEvents.OnBuffApplied += CallApplyMessage;
            BuffEvents.OnBuffExpired += CallExpireMessage;
            PlayerEvent.OnTakeDamage += CallDamageMessage;
            SetGoldForDebug();
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

        void SetGoldForDebug()
        {
            GoldManager.Instance.AddGold(100000);
        }
    }
}
