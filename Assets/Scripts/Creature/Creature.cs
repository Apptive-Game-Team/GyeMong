using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : MonoBehaviour
{
    public enum State
    {
        READY,
        MOVE,
        ATTACK
    }
    public enum StatusEffect
    {
        NONE,
        CHANGINGPATTERN,
        ONHIT,
        STUN
    }
    public State curState;
    public StatusEffect statusEffect;
    protected float maxHealth;
    [SerializeField] protected float curHealth;
    protected float AttackDamage;
    protected float speed;
    protected float detectionRange;
    protected float MeleeAttackRange;
    protected float RangedAttackRange;
    [SerializeField] protected float shield;
    protected GameObject player;
    public virtual void TakeDamage(float damage)
    {
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
    protected virtual void ChageState(State newState)
    {
        curState = newState;
    }
    protected virtual void ChageStateEffect()
    {

    }
}

