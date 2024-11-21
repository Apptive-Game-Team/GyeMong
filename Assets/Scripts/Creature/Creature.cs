using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : MonoBehaviour
{
    public enum State
    {
        IDLE,
        MOVE,
        ATTACK,
        CHANGINGPATTERN,
        ONHIT,
        STUN
    }
    private FiniteStateMachine _fsm;
    public State _curState;
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
    protected virtual void ChageState(State nextState)
    {
        _curState = nextState;
        switch(_curState)
        {
            case State.IDLE:
                {
                    _fsm.ChangeState(new IdleState(this)); 
                    break;
                }
            case State.MOVE:
                {
                    _fsm.ChangeState(new MoveState(this));
                    break;
                }
            case State.ATTACK:
                {
                    _fsm.ChangeState(new AttackState(this));
                    break;
                }
        }
    }
}

