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
    protected bool onBattle;//디버깅용, 디텍터 버그 해결시 false로 전환
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

