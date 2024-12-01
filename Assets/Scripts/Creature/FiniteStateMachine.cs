using UnityEngine.XR;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    protected Animator animator;
    protected Creature _creature;

    protected BaseState(Creature creature)
    {
        _creature = creature;
        animator = creature.GetComponent<Animator>();
    }
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
public class NoneState : BaseState
{
    public NoneState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
    }
    public override void OnStateExit()
    {
    }
    public override void OnStateUpdate()
    {
    }
}
public class IdleState : BaseState
{
    public IdleState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
    }
    public override void OnStateExit()
    {
    }
    public override void OnStateUpdate()
    {
        animator.SetFloat("xDir", _creature.moveDir.x);
        animator.SetFloat("yDir", _creature.moveDir.y);
    }
}
public class MoveState : BaseState
{
    public MoveState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
        animator.SetBool("isMove", true);
    }
    public override void OnStateExit()
    {
        animator.SetBool("isMove", false);
    }
    public override void OnStateUpdate()
    {
        animator.SetFloat("xDir", _creature.moveDir.x);
        animator.SetFloat("yDir", _creature.moveDir.y);
    }
}
public class AttackState : BaseState
{
    public AttackState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
        animator.SetBool("isAttack", true);
    }
    public override void OnStateExit()
    {
        animator.SetBool("isAttack", false);
    }
    public override void OnStateUpdate()
    {
        animator.SetFloat("patternType", _creature.curPattern);
        animator.SetFloat("xDir", _creature.moveDir.x);
        animator.SetFloat("yDir", _creature.moveDir.y);
    }
}
public class ChangingPatternState : BaseState
{
    public ChangingPatternState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
        animator.SetBool("isChangePattern", true);
    }
    public override void OnStateExit()
    {
        animator.SetBool("isChangePattern", false);
    }
    public override void OnStateUpdate()
    {
        animator.SetFloat("patternType", _creature.curPattern);
        animator.SetFloat("xDir", _creature.moveDir.x);
        animator.SetFloat("yDir", _creature.moveDir.y);
    }
}
public class OnHitState : BaseState
{
    public OnHitState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
    }
    public override void OnStateExit()
    {
    }
    public override void OnStateUpdate()
    {
    }
}
public class StunState : BaseState
{
    public StunState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
    }
    public override void OnStateExit()
    {
    }
    public override void OnStateUpdate()
    {
    }
}
public class DashState : BaseState
{
    public DashState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
        animator.SetBool("isDash", true);
    }
    public override void OnStateExit()
    {
        animator.SetBool("isDash", false);
    }
    public override void OnStateUpdate()
    {
        animator.SetFloat("xDir", _creature.moveDir.x);
        animator.SetFloat("yDir", _creature.moveDir.y);
    }
}
public class FiniteStateMachine
{
    public FiniteStateMachine(BaseState initState)
    {
        _curState = initState;
        ChangeState(_curState);
    }
    private BaseState _curState;
    public void ChangeState(BaseState nextState)
    {
        if(nextState == _curState) 
        {
            return;
        }
        if(_curState != null) 
        {
            _curState.OnStateExit();
        }
        _curState = nextState;
        _curState.OnStateEnter();
    }
    public void UpdateState()
    {
        if(_curState != null) 
        {
            _curState.OnStateUpdate();
        }
    }
}