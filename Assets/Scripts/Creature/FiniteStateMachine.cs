using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseState
{
    protected Creature _creature;

    protected BaseState(Creature creature)
    {
        _creature = creature;
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
    }
}
public class MoveState : BaseState
{
    public MoveState(Creature creature) : base(creature) { }
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
public class AttackState : BaseState
{
    public AttackState(Creature creature) : base(creature) { }
    public override void OnStateEnter()
    {
    }
    public override void OnStateExit()
    {
    }
    public override void OnStateUpdate()
    {
        Debug.Log("원거리 공격");
    }
}
public class ChangingPatternState : BaseState
{
    public ChangingPatternState(Creature creature) : base(creature) { }
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