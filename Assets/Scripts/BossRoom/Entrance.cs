using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public MidBoss midBoss;

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("rkskek");
            if (midBoss != null)
            {
                midBoss.StartDetectingPlayer();
            }
        }
    }
}

