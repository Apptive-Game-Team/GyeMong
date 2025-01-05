using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringGuardianBossRoomEntrance : MonoBehaviour, IEventTriggerable
{
    [SerializeField] Golem guardian;
    [SerializeField] GameObject rootPatternManger;
    private void Start()
    {
        if (ConditionManager.Instance.Conditions.TryGetValue("spring_guardian_down", out bool down))
        {
            if (down)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Trigger()
    {
        if (guardian != null)
        {
            // guardian.StartDetectingPlayer();
        }
        if(rootPatternManger != null)
        {
            rootPatternManger.SetActive(true);
        }
        Destroy(gameObject);
    }
}

