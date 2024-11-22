using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringGuardianBossRoomEntrance : MonoBehaviour
{
    [SerializeField] Guardian guardian;
    [SerializeField] GameObject rootPatternManger;
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("rkskek");
            if (guardian != null)
            {
                guardian.StartDetectingPlayer();
            }
            if(rootPatternManger != null)
            {
                rootPatternManger.SetActive(true);
            }
        }
    }
}

