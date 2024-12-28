using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossRoomEntrance : MonoBehaviour, IEventTriggerable
{
    [SerializeField] MidBoss midBoss;
    
    private void Start()
    {
        if (ConditionManager.Instance.Conditions.TryGetValue("spring_midboss_down", out bool down))
        {
            if (down)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Trigger()
    { 
        if (midBoss != null && midBoss.curState == Creature.State.NONE)
        {
            midBoss.StartDetectingPlayer();
        }
    }
}

