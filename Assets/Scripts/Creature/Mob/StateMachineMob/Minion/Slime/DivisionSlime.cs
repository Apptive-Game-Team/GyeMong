using System.Collections;
using System.Game;
using Creature.Mob.StateMachineMob.Minion.Component.detector;
using Creature.Mob.StateMachineMob.Minion.Component.pathfinder;
using Creature.Mob.StateMachineMob.Minion.Slime.Components;
using DG.Tweening;
using playerCharacter;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Minion.Slime
{
    public class DivisionSlime : SlimeBase
    {
        private const float DIVIDE_RATIO = 0.6f;
        private int _divisionLevel = 0;
        private int _maxDivisionLevel = 2;
        
        protected override void Start()
        {
            Initialize();
            DivisionSlimeManager.Instance.RegisterSlime(this);
        }

        protected override void Initialize()
        {
            maxHp = 10f;
            currentHp = maxHp;
            speed = 2f;
            
            _detector = SimplePlayerDetector.Create(this);
            _pathFinder = new SimplePathFinder();
            _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
            
            MeleeAttackRange = 3f;
            RangedAttackRange = 10f;
            detectionRange = 20f;

            damage = 10f;
        }

        public override void StartMob()
        {
            StartCoroutine(FaceToPlayer());
            ChangeState();
        }

        public override void OnAttacked(float damage)
        {
            base.OnAttacked(damage);
            if (currentState is not SlimeDieState)
            {
                StopCoroutine(_currentStateCoroutine);
                StartCoroutine(GetComponent<AirborneController>().AirborneTo(transform.position - DirectionToPlayer * MeleeAttackRange));
                ChangeState();
            }
        }

        public class SlimeRangedAttackState : RangedAttackState { }
        public class SlimeMeleeAttackState : MeleeAttackState { }
        
        public class DashAttackState : SlimeState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                return (DivisionSlime.DistanceToPlayer > DivisionSlime.MeleeAttackRange &&
                        DivisionSlime.DistanceToPlayer < DivisionSlime.RangedAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MeleeAttack, true);
                float dashDistance = 1.5f;
                Vector3 dashTargetPosition = PlayerCharacter.Instance.transform.position + DivisionSlime.DirectionToPlayer * dashDistance;
                yield return new WaitForSeconds(2 * SlimeAnimator.AnimationDeltaTime);
                DivisionSlime.transform.DOMove(dashTargetPosition, 0.3f).SetEase(Ease.OutQuad);
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                yield return new WaitForSeconds(1);
                DivisionSlime.ChangeState();
            }
        }

        public class DieState : SlimeDieState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public DieState() { }
            public DieState(StateMachineMob mob)
            {
                this.mob = mob;
            }
            public override int GetWeight()
            {
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                if (DivisionSlime._divisionLevel < DivisionSlime._maxDivisionLevel)
                {
                    DivisionSlime.Divide();
                }
                else
                {
                    DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Die);
                    yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                }
                Destroy(DivisionSlime.gameObject);
            }
        }

        protected override void OnDead()
        {
            ChangeState(new DieState(this));
            DivisionSlimeManager.Instance.UnregisterSlime(this);
        }

        public class MoveState : SlimeMoveState { }

        private void Divide()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 spawnPosition = transform.position;
                int attempts = 10;
                do
                {
                    Vector3 candidatePosition = transform.position + Random.insideUnitSphere;
                    candidatePosition.z = 0f;
                    var hit = Physics2D.Raycast(transform.position, candidatePosition - transform.position, Vector3.Distance(transform.position, candidatePosition), LayerMask.GetMask("Wall"));
                    if (hit.collider == null)
                    {
                        spawnPosition = candidatePosition;
                        break;
                    }
                } while (attempts-- > 0);
                
                GameObject newSlime = Instantiate(gameObject, transform.position, Quaternion.identity);
                newSlime.transform.localScale = transform.localScale * DIVIDE_RATIO;
                
                newSlime.transform.DOJump(spawnPosition, 1f, 1, 0.5f).SetEase(Ease.OutQuad);

                DivisionSlime slimeComponent = newSlime.GetComponent<DivisionSlime>();
                
                slimeComponent._slimeAnimator = SlimeAnimator.Create(slimeComponent.gameObject, sprites);
                slimeComponent.damage *= DIVIDE_RATIO;
                slimeComponent.MeleeAttackRange *= DIVIDE_RATIO;
                slimeComponent.RangedAttackRange *= DIVIDE_RATIO;
                slimeComponent._divisionLevel = _divisionLevel + 1;
                slimeComponent.ChangeState(new SlimeMoveState(slimeComponent));
                
                DivisionSlimeManager.Instance.RegisterSlime(slimeComponent);
            }
        }
    }
}