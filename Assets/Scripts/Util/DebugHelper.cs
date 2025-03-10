using System.Collections;
using System.Collections.Generic;
using System.Game.Buff;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    void Start()
    {
        BuffEvents.OnBuffApplied += ShowMessage;
    }

    void ShowMessage(BuffData buff, IBuffable buffable)
    {
        Debug.Log($"{buffable} Recieved Buff {buff}");
    }
}
