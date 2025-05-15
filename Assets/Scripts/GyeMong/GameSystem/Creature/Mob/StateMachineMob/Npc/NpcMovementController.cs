using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.pathfinder;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Npc
{
    public class NpcMovementController : StateMachineMob, IControllable, IEventTriggerable
    {
        private IPathFinder _pathFinder = new SimplePathFinder();
        private const int MAX_DISTANCE_TO_PLAYER = 2;
        private const int PATH_FINDING_INTERVAL = 1;

        private void Start()
        {
            speed = 3;
            ChangeState(new NpcIdleState() { mob = this });;
        }

        public class NpcIdleState : NpcState
        {
            public override IEnumerator StateCoroutine()
            {
                yield return null;
            }
        }
        
        public class NpcFollowingState : NpcState
        {
            private Vector3 _target;
            private List<Vector2> _path;
            private Coroutine _pathFindingCoroutine;

            private IEnumerator PathFindingCoroutine()
            {
                while (true)
                {
                    _path = Npc._pathFinder.FindPath(mob.transform.position, PlayerCharacter.Instance.transform.position);
                    UpdateTarget();
                    yield return new WaitForSeconds(PATH_FINDING_INTERVAL);
                }
            }
            
            private void UpdateTarget()
            {
                if (_path.Count > 0)
                {
                    _target = _path[0];
                    _path.RemoveAt(0);
                    if ((_target - mob.transform.position).magnitude < 0.1f)
                    {
                       UpdateTarget();
                    }
                }
            }
            
            public override IEnumerator StateCoroutine()
            {
                mob.transform.parent = PlayerCharacter.Instance.transform;
                _pathFindingCoroutine = mob.StartCoroutine(PathFindingCoroutine());
                
                while (true)
                {
                    if (mob.DistanceToPlayer < MAX_DISTANCE_TO_PLAYER)
                    { // close to Player
                        Npc.Animator.SetBool("walking", false);
                    }
                    else
                    { // far from Player
                        Npc.Animator.SetBool("walking", true);
                        mob.transform.position = Vector3.MoveTowards(mob.transform.position, _target, mob.speed * Time.deltaTime);
                        Vector3 direction = _target - mob.transform.position;
                        
                        Npc.Animator.SetFloat("x", direction.x);
                        Npc.Animator.SetFloat("y", direction.y);
                    }
                    yield return null;
                }
            }
            
            public override void OnStateExit()
            {
                mob.StopCoroutine(_pathFindingCoroutine);
                mob.transform.parent = null;
            }
        }
        
        public abstract class NpcState : BaseState // is not use with random
        {
            protected NpcMovementController Npc { get { return (NpcMovementController)mob; } }
            public override int GetWeight()
            {
                return 0;
            }
        }
        
        
        public IEnumerator MoveTo(Vector3 target, float speed)
        {
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }
        }

        public void Trigger() // toggle Npc's state
        {
            if (currentState is NpcIdleState)
            {
                ChangeState(new NpcFollowingState() { mob = this });
            }
            else
            {
                ChangeState(new NpcIdleState() { mob = this });
            }
        }
    }
}
