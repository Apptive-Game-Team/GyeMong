using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : MonoBehaviour
{
    protected float maxHealth;
    [SerializeField] protected float curHealth;
    protected float AttackDamage;
    protected float speed;
    protected float detectionRange;
    protected float MeleeAttackRange;
    protected float RangedAttackRange;
    [SerializeField] protected float shield;
    protected GameObject player;
    protected bool onBattle = false;
    public virtual void TakeDamage(float damage)
    {
        //curHealth -= damage;
        if (shield >= damage)
        {
            shield -= damage;
        }
        else
        {
            float temp = shield;
            shield = 0;
            curHealth -= (damage-temp);
        }
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

