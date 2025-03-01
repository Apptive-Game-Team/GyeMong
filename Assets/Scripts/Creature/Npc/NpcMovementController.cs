using System.Collections;
using System.Collections.Generic;
using System.Event.Interface;
using playerCharacter;
using UnityEngine;

namespace Creature.Npc
{
    public class NpcMovementController : Creature, IControllable, IEventTriggerable
    {
        private IPathFinder _pathFinder = new SimplePathFinder();
        private const int MAX_DISTANCE_TO_PLAYER = 2;
        private const int PATH_FINDING_INTERVAL = 1;

        private void Start()
        {
            speed = 3;
            ChangeState(new NpcIdleState() { creature = this });;
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
                    _path = Npc._pathFinder.FindPath(creature.transform.position, PlayerCharacter.Instance.transform.position);
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
                    if ((_target - creature.transform.position).magnitude < 0.1f)
                    {
                       UpdateTarget();
                    }
                }
            }
            
            public override IEnumerator StateCoroutine()
            {
                creature.transform.parent = PlayerCharacter.Instance.transform;
                _pathFindingCoroutine = creature.StartCoroutine(PathFindingCoroutine());
                
                while (true)
                {
                    if (creature.DistanceToPlayer < MAX_DISTANCE_TO_PLAYER)
                    { // close to Player
                        
                    }
                    else
                    { // far from Player
                        creature.transform.position = Vector3.MoveTowards(creature.transform.position, _target, creature.speed * Time.deltaTime);
                    }
                    yield return null;
                }
            }
            
            public override void OnStateExit()
            {
                creature.StopCoroutine(_pathFindingCoroutine);
                creature.transform.parent = null;
            }
        }
        
        public abstract class NpcState : BaseState // is not use with random
        {
            protected NpcMovementController Npc { get { return (NpcMovementController)creature; } }
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
                ChangeState(new NpcFollowingState() { creature = this });
            }
            else
            {
                ChangeState(new NpcIdleState() { creature = this });
            }
        }
    }
}
