using System.Collections;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer
{
    public class Wanderer : StateMachineMob
    {
        protected IDetector<PlayerCharacter> _detector;
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private GameObject skillPrefab;

        public override void OnAttacked(float damage)
        {
            currentHp -= damage;
            if (currentHp <= 0)
            {
                OnDead();
            }
            else
            {
                StartCoroutine(Blink());
            }
        }

        protected override void OnDead()
        {
            StageManager.ClearStage(this);
        }

        private void Start()
        {
            Initialize();

            ChangeState(new DetectingPlayer() {mob= this});
        }

        private IEnumerator MeleeAttack()
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * 0.5f, 
                    DirectionToPlayer, 
                    attackPrefab, 
                    new StaticMovement(
                        transform.position + DirectionToPlayer * 0.5f, 
                        0.3f)
                )
                .StartRoutine();
            yield return new WaitForSeconds(0.2f);
            _animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator RangeAttack()
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * 0.5f, 
                    DirectionToPlayer, 
                    attackPrefab, 
                    new StaticMovement(
                        transform.position + DirectionToPlayer * 0.5f, 
                        0.3f)
                )
                .StartRoutine();
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * 0.8f, 
                    DirectionToPlayer, 
                    skillPrefab, 
                    new LinearMovement(
                        transform.position, 
                        transform.position + DirectionToPlayer * 10, 
                        10)
                )
                .StartRoutine();
            yield return new WaitForSeconds(0.2f);
            _animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.1f);
        }

        protected void Initialize()
        {
            maxHp = 30;
            currentHp = maxHp;

            currentShield = 0;
            damage = 10;
            speed = 1;
            detectionRange = 10;
            MeleeAttackRange = 2;
            RangedAttackRange = 20;

            _detector = SimplePlayerDistanceDetector.Create(this);
        }

        private void FaceToPlayer()
        {
            Animator.SetFloat("xDir", DirectionToPlayer.x);
            Animator.SetFloat("yDir", DirectionToPlayer.y);
        }

        public class CircularSlash : WandererState
        {
            public override int GetWeight()
            {
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class HeavyTripleSlash : WandererState
        {
            public override int GetWeight()
            {
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class UpwardSlashWithStun : WandererState
        {
            public override int GetWeight()
            {
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class GroundSmash : WandererState
        {
            public override int GetWeight()
            {
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class DetectingPlayer : WandererState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                mob.Animator.SetBool("isMove", false);
                while (true)
                {
                    Transform target = Wanderer._detector.DetectTarget()?.transform;
                    if (target != null)
                    {
                        mob.ChangeState();
                        yield break;
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        
        public abstract class WandererState : BaseState
        {
            protected Wanderer Wanderer => mob as Wanderer;
        }
    }
}
