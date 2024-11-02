using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : MonoBehaviour
{
    protected float maxHealth;
    protected float curHealth;
    protected float speed;
    protected float detectionRange = 1f;
    protected GameObject player;
    protected bool onBattle = false;
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
}

