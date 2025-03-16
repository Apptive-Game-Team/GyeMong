using System.Collections;
using System.Collections.Generic;
using System.Game.Buff;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    void Start()
    {
        BuffEvents.OnBuffApplied += CallApplyMessage;
        BuffEvents.OnBuffExpired += CallExpireMessage;
    }

    void CallApplyMessage(BuffData buff, IBuffable buffable)
    {
        Debug.Log($"{buffable} Apply Buff {buff}");
    }
    void CallExpireMessage(BuffData buff, IBuffable buffable)
    {
        Debug.Log($"{buffable} Expire Buff {buff}");
    }
}
