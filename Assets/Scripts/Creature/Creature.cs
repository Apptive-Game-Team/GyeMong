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
        STUN,
        DASH,
        CHANGINGPHASE
    }
    protected FiniteStateMachine _fsm;
    public State curState;
    public int curPattern;
    public int dashType;
    protected float maxHealth;
    [SerializeField] protected float curHealth;
    public float defaultDamage;
    protected float speed;
    protected float detectionRange;
    protected float MeleeAttackRange;
    protected float RangedAttackRange;
    public Vector2 moveDir { get; private set; }
    [SerializeField] protected float shield;
    protected GameObject player;
    protected virtual void Update()
    {
        Vector3 targetPosition = player.transform.position;
        Vector3 direction = targetPosition - transform.position;
        SetMoveDirection(new Vector2(direction.x, direction.y));
    }
    public void SetMoveDirection(Vector2 direction)
    {
        moveDir = direction;
        moveDir.Normalize();
    }
    public virtual void TakeDamage(float damage)
    {
        OnHit();
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
    protected virtual void Die() // Clear Event
    {
        try
        {
            GameObject.Find("BossDownEventObject").gameObject.GetComponent<EventObject>().Trigger();
        }
        catch
        {
            Debug.Log("BossDownEventObject not found");
        }
        Destroy(gameObject);
    }
    public virtual void ChangeState(State nextState)
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
            case State.DASH:
                {
                    _fsm.ChangeState(new DashState(this));
                    break;
                }
            case State.CHANGINGPHASE:
                {
                    _fsm.ChangeState(new ChangingPhaseState(this));
                    break;
                }
        }
    }
    public void OnHit()
    {
        StartCoroutine(OnHitCoroutine());
    }

    private IEnumerator OnHitCoroutine()
    {
        State curState_ = curState;
        ChangeState(State.ONHIT);
        yield return null;
        ChangeState(curState_);
    }
}

