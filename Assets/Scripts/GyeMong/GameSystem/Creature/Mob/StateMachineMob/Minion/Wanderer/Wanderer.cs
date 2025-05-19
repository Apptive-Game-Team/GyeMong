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
        [SerializeField] private GameObject basicAttackPrefab;
        [SerializeField] private GameObject upwardSlashPrefab;
        [SerializeField] private GameObject circleSlashPrefab;
        [SerializeField] private GameObject attackFloorPrefab;
        [SerializeField] private GameObject comboSlashPrefab;
        [SerializeField] private GameObject growFloorPrefab;

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

        private IEnumerator StaticAttack(GameObject prefab, float distance = 0.5f, float duration = 0.5f)
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * distance, 
                    DirectionToPlayer, 
                    prefab, 
                    new StaticMovement(
                        transform.position + DirectionToPlayer * distance, 
                        duration)
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
                if (mob.DistanceToPlayer > mob.MeleeAttackRange)
                {
                    return 0;
                }
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return Wanderer.StaticAttack(Wanderer.circleSlashPrefab, 0);
                yield return new WaitForSeconds(1f);
                Wanderer.ChangeState(new DetectingPlayer() {mob = Wanderer});
            }
        }
        
        public class HeavyTripleSlash : WandererState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer > mob.MeleeAttackRange)
                {
                    return 0;
                }
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return Wanderer.StaticAttack(Wanderer.basicAttackPrefab);
                yield return Wanderer.StaticAttack(Wanderer.basicAttackPrefab);
                yield return Wanderer.StaticAttack(Wanderer.comboSlashPrefab);
                yield return new WaitForSeconds(1f);
                Wanderer.ChangeState(new DetectingPlayer() {mob = Wanderer});
            }
        }
        
        public class UpwardSlashWithStun : WandererState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer > mob.MeleeAttackRange)
                {
                    return 0;
                }
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return Wanderer.StaticAttack(Wanderer.upwardSlashPrefab);
                yield return new WaitForSeconds(1f);
                Wanderer.ChangeState(new DetectingPlayer() {mob = Wanderer});
            }
        }
        
        public class GroundSmash : WandererState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer > mob.RangedAttackRange)
                {
                    return 0;
                }
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                Debug.Log("Ground Smash");
                yield return Wanderer.StaticAttack(Wanderer.attackFloorPrefab);
                yield return Wanderer.StaticAttack(Wanderer.growFloorPrefab, 5f, 100f);
                yield return new WaitForSeconds(1f);
                Wanderer.ChangeState(new DetectingPlayer() {mob = Wanderer});
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
                Debug.Log("Detecting Player");
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
        
        public class WalkState : WandererState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer > mob.MeleeAttackRange)
                {
                    return 5;
                }
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                mob.Animator.SetBool("isMove", true);
                while (mob.DistanceToPlayer > mob.MeleeAttackRange)
                {
                    mob.TrackPlayer();
                    Wanderer.FaceToPlayer();
                    yield return null;
                }
                mob.Animator.SetBool("isMove", false);
                yield return null;
                mob.ChangeState();
            }
        }
        
        public abstract class WandererState : BaseState
        {
            protected Wanderer Wanderer => mob as Wanderer;
        }
    }
}
