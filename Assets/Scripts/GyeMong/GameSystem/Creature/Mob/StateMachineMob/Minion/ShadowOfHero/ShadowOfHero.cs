using System.Collections;
using GyeMong.EventSystem.Event;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Indicator;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class ShadowOfHero : StateMachineMob
    {
        private int _attackCount = 0;
        private const int MAX_ATTACK_COUNT = 3;
        protected IDetector<PlayerCharacter> _detector;
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private GameObject skillPrefab;
        [SerializeField] private GameObject key;
        [SerializeField] private SkllIndicatorDrawer SkillIndicator;

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
            StartCoroutine(DeadTrigger());
        }

        private void Start()
        {
            Initialize();
            SkillIndicator = transform.Find("SkillIndicator").GetComponent<SkllIndicatorDrawer>();
            // for debug

            //ChangeState(new DetectingPlayer() {mob= this});
        }

        private IEnumerator MeleeAttack()
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, DirectionToPlayer);
            StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator
                    (attackPrefab, transform.position + DirectionToPlayer, rotation, 0.3f,
                        () =>
                            AttackObjectController.Create(
                                transform.position + DirectionToPlayer * 0.5f,
                                DirectionToPlayer,
                                attackPrefab,
                                new StaticMovement(
                                    transform.position + DirectionToPlayer * 0.5f, 0.3f)
                               ).StartRoutine()
                    ));
            yield return new WaitForSeconds(0.5f);
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
            maxHp = 100;
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

        public class DetectingPlayer : ShadowState
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
                    Transform target = (mob as ShadowOfHero)._detector.DetectTarget()?.transform;
                    if (target != null)
                    {
                        mob.ChangeState();
                        yield break;
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        
        public class WalkState : ShadowState
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
                     ShadowOfHero.FaceToPlayer();
                     yield return null;
                }
                mob.Animator.SetBool("isMove", false);
                yield return null;
                mob.ChangeState();
            }
        }
        
        public class ComboAttackState : ShadowState
        {
            public override int GetWeight()
            {
                if (ShadowOfHero._attackCount >= MAX_ATTACK_COUNT && mob.DistanceToPlayer < mob.MeleeAttackRange)
                {
                    ShadowOfHero._attackCount = 0;
                    return 100;
                }
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                ShadowOfHero.SkillIndicator.DrawIndicator(SkllIndicatorDrawer.IndicatorType.Line, ShadowOfHero.SkillIndicator.transform.position, SceneContext.Character.transform, 0.5f, 0.4f);
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < 10; i++)
                {
                    yield return ShadowOfHero.RangeAttack();
                }
                mob.Animator.SetBool("isHuck", true);
                yield return new WaitForSeconds(5f); // Exhausted Delay
                mob.Animator.SetBool("isHuck", false);
                mob.ChangeState();
            }
        }
       
        public class AttackState : ShadowState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer < mob.MeleeAttackRange)
                    return 5;
                return 0;
            }
            
            public override IEnumerator StateCoroutine()
            {
                ShadowOfHero._attackCount += 1;
                yield return ShadowOfHero.MeleeAttack();
                mob.ChangeState();
            }
        }
        
        public class DashAttackState : ShadowState
        {
            public override int GetWeight()
            {
                if (mob.DistanceToPlayer > mob.MeleeAttackRange && mob.DistanceToPlayer < mob.RangedAttackRange)
                {
                    return 5;
                }
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                ShadowOfHero._attackCount += 1;
                yield return mob.RushAttack(0.5f);
                yield return ShadowOfHero.MeleeAttack();
                mob.ChangeState();
            }
        }

        public abstract class ShadowState : BaseState
        {
            protected ShadowOfHero ShadowOfHero => mob as ShadowOfHero;
        }

        private IEnumerator DeadTrigger()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            StopCoroutine(_currentStateCoroutine);
            yield return (new DropObjectEvent() { _gameObject = key, _position = transform.position}).Execute();
            yield return new WaitForSecondsRealtime(1f);
            StageManager.ClearStage(this);
        }
    }
}
