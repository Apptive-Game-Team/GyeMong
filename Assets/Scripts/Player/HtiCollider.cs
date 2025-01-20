using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HtiCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log("Attacked!");
            GrazeController.Instance.colliderAttackedMap[other] = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("EnemyAttack"))
        {
            Debug.Log("Attacked!");
            GrazeController.Instance.colliderAttackedMap[other.collider] = true;
        }
    }
}
