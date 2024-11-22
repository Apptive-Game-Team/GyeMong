using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Creature : MonoBehaviour
{
    public enum State
    {
        NONE,
        IDLE,
        MOVE,
        ATTACK,
        CHANGINGPATTERN,
        ONHIT,
        STUN
    }
    protected FiniteStateMachine _fsm;
    public State curState;
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
    protected virtual void ChangeState(State nextState)
    {
        curState = nextState;
        switch(curState)
        {
            case State.NONE:
                {
                    _fsm.ChangeState(new NoneState(this));
                    break;
                }
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
            case State.CHANGINGPATTERN:
                {
                    _fsm.ChangeState(new ChangingPatternState(this));
                    break;
                }
            case State.ONHIT:
                {
                    _fsm.ChangeState(new OnHitState(this));
                    break;
                }
            case State.STUN:
                {
                    _fsm.ChangeState(new StunState(this));
                    break;
                }
        }
    }
}

