using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBossMeleeAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDemo.Instance.TakeDamage(10);
        }
    }
}
