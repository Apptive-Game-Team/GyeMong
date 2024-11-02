using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public MidBoss midBoss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (midBoss != null)
            {
                midBoss.StartDetectingPlayer();
            }
        }
    }
}

