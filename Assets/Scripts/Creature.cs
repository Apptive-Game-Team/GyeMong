using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public float speed;
    public float detectionRange = 10f;
    private Transform playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public bool DetectPlayer()
    {
        if (playerTransform == null)
        {
            return false;
        }
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= detectionRange;
    }
}

