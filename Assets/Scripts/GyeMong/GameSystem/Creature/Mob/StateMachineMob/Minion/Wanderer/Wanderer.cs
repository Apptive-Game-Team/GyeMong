using System.Collections;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Direction;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Indicator;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer
{
    public class Wanderer : StateMachineMob
    {
        private const float DEFAULT_ANGULAR_VELOCITY = 1f; // radians per second
        private const float FAST_ANGULAR_VELOCITY = Mathf.PI; // radians per second
        
        protected IDetector<PlayerCharacter> _detector;
        [SerializeField] private GameObject basicAttackPrefab;
        [SerializeField] private GameObject upwardSlashPrefab;
        [SerializeField] private GameObject circleSlashPrefab;
        [SerializeField] private GameObject attackFloorPrefab;
        [SerializeField] private GameObject comboSlashPrefab;
        [SerializeField] private GameObject growFloorPrefab;

        private DirectionController _directionController;

        public override void OnAttacked(float damage)
        {
            if (!IsPlayerAtBack())
            {
                StartCoroutine(CounterAttack());
                return;
            }
            
            currentHp -= damage;
            Debug.Log("Attack Complete");

            StartCoroutine(OnAttackedReact());

            if (currentHp <= 0)
            {
                OnDead();
            }
            else
            {
                StartCoroutine(Blink());
            }
        }

        private IEnumerator CounterAttack()
        {
            _directionController.SetAngularVelocity(FAST_ANGULAR_VELOCITY);

            SoundObject _soundObject;
            int randomSuffix = UnityEngine.Random.Range(2, 4);
            string soundName = "ENEMY_Wanderer_CounterAttack" + randomSuffix;
            _soundObject = Sound.Play(soundName, false);

            yield return new WaitForSeconds(0.2f);
            yield return StaticChildAttack(comboSlashPrefab);
            _directionController.SetAngularVelocity(DEFAULT_ANGULAR_VELOCITY);
        }
        private IEnumerator OnAttackedReact()
        {
            StartCoroutine(Knockback());
            SceneContext.CameraManager.CameraShake(0.2f);
            yield return null;
        }
        private IEnumerator Knockback(float distance = 0.5f, float duration = 0.1f)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + _directionController.GetDirection() * distance;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
        }

        private bool IsPlayerAtBack()
        {
            float angle = _directionController.GetAngleDifference(SceneContext.Character.transform);
            return Mathf.Abs(angle) > Mathf.PI * 3 / 4;
        }

        private void Awake()
        {
            _directionController = GetComponent<DirectionController>();
            _materialController = GetComponent<MaterialController>();
            StartCoroutine(_directionController.TrackPlayer(DEFAULT_ANGULAR_VELOCITY, true));
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

        private void Update()
        {
            if (IsPlayerAtBack())
                _materialController.SetMaterial(MaterialController.MaterialType.BACK);
            else
                _materialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
        }

        private IEnumerator StaticChildAttack(GameObject prefab, float distance = 0.5f, float duration = 0.5f, float delay = 0.3f)
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            yield return ApplyAttackingMove(0.2f);
            yield return IndicatorGenerator.Instance.GenerateIndicator(
                AttackObjectController.Create(
                    transform.position + _directionController.GetDirection() * distance, 
                    _directionController.GetDirection(), 
                    prefab, 
                    new ChildMovement(
                        transform, 
                        _directionController.GetDirection() * distance, 
                        duration)
                ), delay);
            _animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator StaticAttack(GameObject prefab, float distance = 0.5f, float duration = 0.5f)
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            yield return ApplyAttackingMove(0.2f);
            AttackObjectController.Create(
                    transform.position + _directionController.GetDirection() * distance, 
                    _directionController.GetDirection(), 
                    prefab, 
                    new StaticMovement(
                        _directionController.GetDirection() * distance + transform.position, 
                        duration)
                )
                .StartRoutine();
            _animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.1f);
        }

        private IEnumerator ApplyAttackingMove(float duration, float speed = 1)
        {
            Vector3 targetPosition = transform.position + _directionController.GetDirection() * speed;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }

        protected void Initialize()
        {
            maxHp = 10;
            currentHp = maxHp;

            currentShield = 0;
            damage = 10;
            speed = 1;
            detectionRange = 10;
            MeleeAttackRange = 2;
            RangedAttackRange = 3;

            _detector = SimplePlayerDistanceDetector.Create(this);
        }

        private void FaceToPlayer()
        {
            Animator.SetFloat("xDir", _directionController.GetDirection().x);
            Animator.SetFloat("yDir", _directionController.GetDirection().y);
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
                yield return Wanderer.StaticChildAttack(Wanderer.circleSlashPrefab, 0, delay: 0.7f);
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
                yield return Wanderer.StaticChildAttack(Wanderer.basicAttackPrefab, delay: 1f);
                SceneContext.CameraManager.CameraShake(0.1f);
                yield return new WaitForSeconds(0.5f);
                yield return Wanderer.StaticChildAttack(Wanderer.basicAttackPrefab);
                SceneContext.CameraManager.CameraShake(0.1f);
                yield return new WaitForSeconds(0.5f);
                yield return Wanderer.StaticChildAttack(Wanderer.comboSlashPrefab);
                SceneContext.CameraManager.CameraShake(0.3f);
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
                yield return Wanderer.StaticChildAttack(Wanderer.upwardSlashPrefab, delay: 0.6f);
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
                Debug.Log("Ground Smash");
                yield return Wanderer.StaticChildAttack(Wanderer.attackFloorPrefab, delay: 1f);
                SceneContext.CameraManager.CameraShake(0.3f);
                yield return Wanderer.StaticAttack(Wanderer.growFloorPrefab, 1f, 10f);
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
