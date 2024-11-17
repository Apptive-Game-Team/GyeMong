using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringGuardianBossRoomEntrance : MonoBehaviour
{
    [SerializeField] Guardian guardian;
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("rkskek");
            if (guardian != null)
            {
                guardian.StartDetectingPlayer();
            }
        }
    }
}

