using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObjectBreaker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Destroy(other.gameObject);
        }
    }
}
