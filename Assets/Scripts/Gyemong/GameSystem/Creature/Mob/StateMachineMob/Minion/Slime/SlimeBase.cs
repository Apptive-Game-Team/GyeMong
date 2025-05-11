using System.Collections;
using Gyemong.GameSystem.Creature.Attack;
using Gyemong.GameSystem.Creature.Attack.Component.Movement;
using Gyemong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using Gyemong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.pathfinder;
using Gyemong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components;
using Gyemong.GameSystem.Creature.Player;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime
{
    public abstract class SlimeBase : StateMachineMob
    {
        protected bool isInitialized = false;
        
        [SerializeField] private GameObject rangedAttack;

        
        protected IDetector<PlayerCharacter> _detector;
        protected  IPathFinder _pathFinder;
        protected  SlimeAnimator _slimeAnimator;
        [SerializeField] protected  SlimeSprites sprites;
        protected Coroutine _faceToPlayerCoroutine;
        
        public override void StartMob() { }
        
        public override void OnAttacked(float damage)
        {
            if (currentState is not SlimeDieState)
            {
                base.OnAttacked(damage);
                if (currentHp <= 0)
                {
                    OnDead();
                }
            }
        }
        
            
        public IEnumerator FaceToPlayer()
        {
            float scale = Mathf.Abs(transform.localScale.x);
            while (true)
            {
                if (PlayerCharacter.Instance.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-scale, scale, scale);
                }
                else
                {
                    transform.localScale = new Vector3(scale, scale, scale);
                }
                yield return null;
            }
        }

        protected virtual void Start()
        {
            Initialize();
            ChangeState();
            _faceToPlayerCoroutine = StartCoroutine(FaceToPlayer());
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                currentHp = maxHp;
                ChangeState();
                _faceToPlayerCoroutine = StartCoroutine(FaceToPlayer());
            }
        }

        protected virtual void Initialize()
        {
            //currentHp = maxHp;
            currentHp = 3;
            
            currentShield = 0;
            damage = 10;
            speed = 1.5f;
            detectionRange = 7;
            MeleeAttackRange = 1;
            RangedAttackRange = 5;

            _detector = SimplePlayerDistanceDetector.Create(this);
            _pathFinder = new SimplePathFinder();
            _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
            
            isInitialized = true;
        }
        
        public abstract class SlimeState : BaseState
        {
            protected SlimeBase Slime => mob as SlimeBase;
        }

        public class IdleState : SlimeState
        {
            public IdleState() { }
            public IdleState(StateMachineMob mob)
            {
                this.mob = mob;
            }
            public override int GetWeight()
            {
                return 1;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                while (true)
                {
                    Transform target = Slime._detector.DetectTarget()?.transform;
                    if (target != null)
                    {
                        mob.ChangeState(new SlimeMoveState(Slime));
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        public abstract class RangedAttackState : SlimeState
        {
            public override int GetWeight()
            {
                return mob.DistanceToPlayer > mob.MeleeAttackRange && mob.DistanceToPlayer < mob.RangedAttackRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RangedAttack);
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                AttackObjectController.Create(
                    mob.transform.position, 
                    mob.DirectionToPlayer, 
                    Slime.rangedAttack,
                    new LinearMovement(
                        mob.transform.position, 
                        mob.transform.position + mob.DirectionToPlayer * mob.RangedAttackRange, 
                        10f)
                ).StartRoutine();
                
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                yield return new WaitForSeconds(1);
                mob.ChangeState();
            }
        }
    
        public class MeleeAttackState : SlimeState
        {
            public override int GetWeight()
            {
                return mob.DistanceToPlayer <= mob.MeleeAttackRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MeleeAttack);
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 2);
                if (mob.DistanceToPlayer <= mob.MeleeAttackRange)   
                    PlayerCharacter.Instance.TakeDamage(mob.damage);
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                yield return new WaitForSeconds(1);
                mob.ChangeState();
            }
        }
    
        public class SlimeMoveState : SlimeState
        {
            public SlimeMoveState(SlimeBase slime)
            {
                mob = slime;
            }
            protected SlimeMoveState() { }

            public override int GetWeight()
            {
                return mob.DistanceToPlayer > mob.MeleeAttackRange && mob.DistanceToPlayer < mob.DetectionRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MeleeAttack, true);
                float duration = 2f;
                float timer = 0f;
            
                while (duration > timer && mob.DistanceToPlayer > mob.MeleeAttackRange)
                {
                    timer += Time.deltaTime;
                    yield return null;
                    Transform target = Slime._detector.DetectTarget()?.transform;
                    if (target)
                    {
                        Slime.TrackPath(Slime._pathFinder.FindPath(mob.transform.position, target.position));
                    }
                    else
                    {
                        mob.ChangeState(new IdleState(mob));
                    }
                }
            
                Slime._slimeAnimator.Stop();
                mob.ChangeState();
            }
        }
    
        public class SlimeDieState : SlimeState
        {
            public SlimeDieState() { }
            public SlimeDieState(StateMachineMob mob)
            {
                this.mob = mob;
            }
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                // Slime.StopCoroutine(Slime.faceToPlayerCoroutine);
                // ((Slime)creature).faceToPlayerCoroutine = null;
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Die);

                yield return new WaitForSeconds(2f);
                mob.gameObject.SetActive(false);
                yield return null;
            }
        }
        
        protected override void OnDead()
        {
            ChangeState(new SlimeDieState(this));
        }
    
        private void RotateArrowTowardsPlayer(GameObject arrow)
        {
            Vector3 direction = DirectionToPlayer; 
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
