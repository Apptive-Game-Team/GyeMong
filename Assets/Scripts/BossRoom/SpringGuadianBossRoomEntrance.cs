using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringGuardianBossRoomEntrance : MonoBehaviour, IEventTriggerable
{
    [SerializeField] Guardian guardian;
    [SerializeField] GameObject rootPatternManger;
    // public void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (guardian != null && guardian.curState == Creature.State.NONE)
    //         {
    //             guardian.StartDetectingPlayer();
    //         }
    //         if(rootPatternManger != null)
    //         {
    //             rootPatternManger.SetActive(true);
    //         }
    //     }
    // }

    public void Trigger()
    {
        if (guardian != null && guardian.curState == Creature.State.NONE)
        {
            guardian.StartDetectingPlayer();
        }
        if(rootPatternManger != null)
        {
            rootPatternManger.SetActive(true);
        }
    }
}

