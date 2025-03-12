using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

namespace Creature.Minion.Slime
{
    public abstract class SlimeBase : Creature
    {
        private const int GOLD_REWARD = 10;
        
        private bool _isInitialized = false;
        
        [SerializeField] private GameObject rangedAttack;

        
        protected IDetector<PlayerCharacter> _detector;
        protected  IPathFinder _pathFinder;
        protected  SlimeAnimator _slimeAnimator;
        [SerializeField] protected  SlimeSprites sprites;
        protected  Coroutine _faceToPlayerCoroutine;
        
        public override void StartMob() { }
        
        public override void OnAttacked(float damage)
        {
            if (currentState is not SlimeDieState)
            {
                base.OnAttacked(damage);
                if (currentHp <= 0)
                {
                    print(currentState);
                    ChangeState(CreateDieState());
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
            if (_isInitialized)
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
            speed = 2;
            detectionRange = 10;
            MeleeAttackRange = 1;
            RangedAttackRange = 5;

            _detector = SimplePlayerDetector.Create(this);
            _pathFinder = new SimplePathFinder();
            _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
            
            _isInitialized = true;
        }
        
        public abstract class SlimeState : BaseState
        {
            protected SlimeBase Slime => creature as SlimeBase;
        }

        public abstract class RangedAttackState : SlimeState
        {
            public override int GetWeight()
            {
                return creature.DistanceToPlayer > creature.MeleeAttackRange && creature.DistanceToPlayer < creature.RangedAttackRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RANGED_ATTACK);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                GameObject arrow =  Instantiate(Slime.rangedAttack, creature.transform.position, Quaternion.identity, creature.transform);
                arrow.SetActive(true);
                Slime.RotateArrowTowardsPlayer(arrow);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
                yield return new WaitForSeconds(1);
                creature.ChangeState();
            }
        }
    
        public class MeleeAttackState : SlimeState
        {
            public override int GetWeight()
            {
                return creature.DistanceToPlayer <= creature.MeleeAttackRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME * 2);
                if (creature.DistanceToPlayer <= creature.MeleeAttackRange) PlayerCharacter.Instance.TakeDamage(creature.damage);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
                yield return new WaitForSeconds(1);
                creature.ChangeState();
            }
        }
    
        public class SlimeMoveState : SlimeState
        {
            public override int GetWeight()
            {
                return creature.DistanceToPlayer > creature.MeleeAttackRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);
                float duration = 2f;
                float timer = 0f;
            
                while (duration > timer && creature.DistanceToPlayer > creature.MeleeAttackRange)
                {
                    timer += Time.deltaTime;
                    yield return null;
                    creature.TrackPath(Slime._pathFinder.FindPath(creature.transform.position, PlayerCharacter.Instance.transform.position));
                }
            
                Slime._slimeAnimator.Stop();
                creature.ChangeState();
            }
        }
    
        public class SlimeDieState : SlimeState
        {
            public SlimeDieState() { }
            public SlimeDieState(Creature creature)
            {
                this.creature = creature;
            }
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                // Slime.StopCoroutine(Slime.faceToPlayerCoroutine);
                // ((Slime)creature).faceToPlayerCoroutine = null;
                Slime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.DIE);
                
                GoldManager.Instance?.AddGold(GOLD_REWARD);

                yield return new WaitForSeconds(2f);
                creature.gameObject.SetActive(false);
                yield return null;
            }
        }

        protected virtual SlimeDieState CreateDieState()
        {
            return new SlimeDieState(this);
        }
            
        private void RotateArrowTowardsPlayer(GameObject arrow)
        {
            Vector3 direction = DirectionToPlayer; 
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
