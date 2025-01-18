using UnityEngine.XR;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using Rework;
using UnityEngine;


// public class NoneState : BaseState
// {
//     public NoneState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//     }
//     public override void OnStateExit()
//     {
//     }
//     public override void OnStateUpdate()
//     {
//     }
// }
// public class IdleState<T> : BaseState where T : Boss
// {
//     public IdleState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         Boss.GetInstance<T>().currentPatternCoroutine = null;
//     }
//     public override void OnStateExit()
//     {
//     }
//     public override void OnStateUpdate()
//     {
//         _animator.SetFloat("xDir", _creature.moveDir.x);
//         _animator.SetFloat("yDir", _creature.moveDir.y);
//     }
// }
// public class MoveState : BaseState
// {
//     public MoveState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         _animator.SetBool("isMove", true);
//     }
//     public override void OnStateExit()
//     {
//         _animator.SetBool("isMove", false);
//     }
//     public override void OnStateUpdate()
//     {
//         _animator.SetFloat("xDir", _creature.moveDir.x);
//         _animator.SetFloat("yDir", _creature.moveDir.y);
//     }
// }
// public class AttackState : BaseState
// {
//     public AttackState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         _animator.SetBool("isAttack", true);
//     }
//     public override void OnStateExit()
//     {
//         _animator.SetBool("isAttack", false);
//     }
//     public override void OnStateUpdate()
//     {
//         _animator.SetFloat("patternType", _creature.curPattern);
//         _animator.SetFloat("xDir", _creature.moveDir.x);
//         _animator.SetFloat("yDir", _creature.moveDir.y);
//     }
// }
// public class ChangingPatternState : BaseState
// {
//     public ChangingPatternState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         _animator.SetBool("isChangePattern", true);
//     }
//     public override void OnStateExit()
//     {
//         _animator.SetBool("isChangePattern", false);
//     }
//     public override void OnStateUpdate()
//     {
//         _animator.SetFloat("patternType", _creature.curPattern);
//         _animator.SetFloat("xDir", _creature.moveDir.x);
//         _animator.SetFloat("yDir", _creature.moveDir.y);
//     }
// }
// public class OnHitState : BaseState
// {
//     public OnHitState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         _animator.SetBool("onHit", true);
//     }
//     public override void OnStateExit()
//     {
//         _animator.SetBool("onHit", false);
//     }
//     public override void OnStateUpdate()
//     {
//     }
// }
// public class StunState : BaseState
// {
//     public StunState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//     }
//     public override void OnStateExit()
//     {
//     }
//     public override void OnStateUpdate()
//     {
//     }
// }
// public class DashState : BaseState
// {
//     public DashState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         _animator.SetBool("isDash", true);
//     }
//     public override void OnStateExit()
//     {
//         _animator.SetBool("isDash", false);
//     }
//     public override void OnStateUpdate()
//     {
//         _animator.SetFloat("dashType", _creature.dashType);
//         _animator.SetFloat("xDir", _creature.moveDir.x);
//         _animator.SetFloat("yDir", _creature.moveDir.y);
//     }
// }
// public class ChangingPhaseState : BaseState
// {
//     public ChangingPhaseState(Creature creature) : base(creature) { }
//     public override void OnStateEnter()
//     {
//         _animator.SetBool("isChangePhase", true);
//     }
//     public override void OnStateExit()
//     {
//         _animator.SetBool("isChangePhase", false);
//     }
//     public override void OnStateUpdate()
//     {
//         _animator.SetFloat("xDir", _creature.moveDir.x);
//         _animator.SetFloat("yDir", _creature.moveDir.y);
//     }
// }
// public class FiniteStateMachine
// {
//     public FiniteStateMachine(BaseState initState)
//     {
//         _curState = initState;
//         ChangeState(_curState);
//     }
//     private BaseState _curState;
//     public void ChangeState(BaseState nextState)
//     {
//         if(nextState == _curState) 
//         {
//             return;
//         }
//         if(_curState != null) 
//         {
//             _curState.OnStateExit();
//         }
//         _curState = nextState;
//         _curState.OnStateEnter();
//     }
//     public void UpdateState()
//     {
//         if(_curState != null) 
//         {
//             _curState.OnStateUpdate();
//         }
//     }
// }