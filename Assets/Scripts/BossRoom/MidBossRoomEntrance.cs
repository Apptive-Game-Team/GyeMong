using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossRoomEntrance : MonoBehaviour, IEventTriggerable
{
    [SerializeField] MidBoss midBoss;
    // public void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (midBoss != null && midBoss.curState == Creature.State.NONE)
    //         {
    //             midBoss.StartDetectingPlayer();
    //         }
    //     }
    // }

    public void Trigger()
    { 
        if (midBoss != null && midBoss.curState == Creature.State.NONE)
        {
            midBoss.StartDetectingPlayer();
        }
    }
}

