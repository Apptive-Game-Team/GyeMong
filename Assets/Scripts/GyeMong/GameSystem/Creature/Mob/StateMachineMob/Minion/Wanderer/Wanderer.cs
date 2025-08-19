using System.Collections;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;
using DG.Tweening;
using GyeMong.GameSystem.Indicator;

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
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int XDir = Animator.StringToHash("xDir");
        private static readonly int YDir = Animator.StringToHash("yDir");

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
            gameObject.SetActive(false);
        }

        private void Start()
        {
            Initialize();

            ChangeState(new DetectingPlayer() {mob= this});
        }

        private IEnumerator StaticChildAttack(GameObject prefab, Vector3 targetPos, float distance = 0.5f, float duration = 0.5f)
        {
            Vector3 attackDir = (targetPos - transform.position).normalized;
            AttackObjectController.Create(
                    transform.position + attackDir * distance, 
                    attackDir, 
                    prefab, 
                    new ChildMovement(
                        transform, 
                        attackDir * distance, 
                        duration)
                )
                .StartRoutine();
            yield return ApplyAttackingMove(targetPos, duration);
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator StaticAttack(GameObject prefab, Vector3 targetPos, float distance = 0.5f, float duration = 0.5f)
        {
            Vector3 attackDir = (targetPos - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, targetPos - transform.position);
            StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator(prefab, transform.position + attackDir * distance, rotation, 0.3f,
                () =>
                {
                    AttackObjectController.Create(
                            transform.position + attackDir * distance, 
                            attackDir, 
                            prefab, 
                            new StaticMovement(
                                attackDir * distance + transform.position, 
                                duration)
                        )
                        .StartRoutine();
                }));
            yield return new WaitForSeconds(0.1f);
        }

        private IEnumerator ApplyAttackingMove(Vector3 targetPos, float duration)
        {
            yield return transform.DOMove(targetPos, duration).SetEase(Ease.OutCubic).WaitForCompletion();
        }

        protected void Initialize()
        {
            maxHp = 30;
            currentHp = maxHp;

            currentShield = 0;
            damage = 10;
            speed = 1;
            detectionRange = 20;
            MeleeAttackRange = 2;
            RangedAttackRange = 3;

            _detector = SimplePlayerDistanceDetector.Create(this);
        }

        private void FaceToPlayer()
        {
            Animator.SetFloat(XDir, DirectionToPlayer.x);
            Animator.SetFloat(YDir, DirectionToPlayer.y);
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
                Wanderer.FaceToPlayer();
                Vector3 targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(0.5f);
                Wanderer._animator.SetTrigger(IsAttacking);
                yield return Wanderer.StaticAttack(Wanderer.circleSlashPrefab, targetPos);
                Wanderer._animator.SetBool(IsAttacking, false);
                yield return new WaitForSeconds(1f);
                Wanderer.ChangeState(new DetectingPlayer() {mob = Wanderer});
            }
        }
        
        public class HeavyTripleDash : WandererState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer < mob.DetectionRange && mob.DistanceToPlayer > mob.RangedAttackRange)
                {
                    return 5;
                }
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Wanderer.FaceToPlayer();
                Vector3 targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(1f);
                Wanderer._animator.SetTrigger(IsAttacking);
                yield return Wanderer.StaticChildAttack(Wanderer.basicAttackPrefab, targetPos);
                Wanderer._animator.SetBool(IsAttacking, false);
                Wanderer.FaceToPlayer();
                targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(1f);
                Wanderer._animator.SetTrigger(IsAttacking);
                yield return Wanderer.StaticChildAttack(Wanderer.basicAttackPrefab, targetPos);
                Wanderer._animator.SetBool(IsAttacking, false);
                Wanderer.FaceToPlayer();
                targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(1.5f);
                Wanderer._animator.SetTrigger(IsAttacking);
                yield return Wanderer.StaticChildAttack(Wanderer.comboSlashPrefab, targetPos);
                Wanderer._animator.SetBool(IsAttacking, false);
                yield return new WaitForSeconds(1.5f);
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
                Wanderer.FaceToPlayer();
                Vector3 targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(0.5f);
                Wanderer._animator.SetTrigger(IsAttacking);
                yield return Wanderer.StaticAttack(Wanderer.upwardSlashPrefab, targetPos);
                Wanderer._animator.SetBool(IsAttacking, false);
                yield return new WaitForSeconds(1.5f);
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
                Wanderer.FaceToPlayer();
                Vector3 targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(0.5f);
                Wanderer._animator.SetTrigger(IsAttacking);
                yield return Wanderer.StaticAttack(Wanderer.attackFloorPrefab, targetPos);
                Wanderer._animator.SetBool(IsAttacking, false);
                yield return new WaitForSeconds(0.3f);
                SceneContext.CameraManager.CameraShake(0.3f);
                GameObject growFloorAttack = Instantiate(Wanderer.growFloorPrefab, targetPos, Quaternion.identity);
                Destroy(growFloorAttack, 1.5f);
                yield return new WaitForSeconds(1.5f);
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
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }
        
        public class WalkState : WandererState
        {
            public override int GetWeight()
            {
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
