using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : SingletonObject<Creature>
{
    protected float maxHealth;
    [SerializeField] protected float curHealth;
    protected float AttackDamage;
    protected float speed;
    protected float detectionRange;
    protected float MeleeAttackRange;
    protected float RangedAttackRange;
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

