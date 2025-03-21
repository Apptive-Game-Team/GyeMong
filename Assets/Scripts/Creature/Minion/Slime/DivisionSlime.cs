using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using playerCharacter;
using System.Game;

namespace Creature.Minion.Slime
{
    public class DivisionSlime : SlimeBase
    {
        private const float divideRatio = 0.6f;
        private int divisionLevel = 0;
        private int maxDivisionLevel = 2;
        private float airborneRange;

        
        protected override void Start()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            maxHp = 10f;
            currentHp = maxHp;
            
            MeleeAttackRange = 3f;
            RangedAttackRange = 10f;
            airborneRange = 4f;

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

        //public class SlimeIdleState : IdleState { }

        public class SlimeRangedAttackState : RangedAttackState { }
        public class SlimeMeleeAttackState : MeleeAttackState { }
        
        public class DashAttackState : SlimeState
        {
            private DivisionSlime DivisionSlime => creature as DivisionSlime;
            public override int GetWeight()
            {
                return (Slime.DistanceToPlayer > Slime.MeleeAttackRange &&
                Slime.DistanceToPlayer < Slime.RangedAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);
                float dashDistance = 1.5f;
                Vector3 dashTargetPosition = PlayerCharacter.Instance.transform.position + DivisionSlime.DirectionToPlayer * dashDistance;
                yield return new WaitForSeconds(2 * SlimeAnimator.ANIMATION_DELTA_TIME);
                DivisionSlime.transform.DOMove(dashTargetPosition, 0.3f).SetEase(Ease.OutQuad);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
                yield return new WaitForSeconds(1);
                DivisionSlime.ChangeState();
            }
        }

        public class DieState : SlimeDieState
        {
            private DivisionSlime DivisionSlime => creature as DivisionSlime;
            public DieState() { }
            public DieState(Creature creature)
            {
                this.creature = creature;
            }
            public override int GetWeight()
            {
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                if (DivisionSlime.divisionLevel < DivisionSlime.maxDivisionLevel)
                {
                    DivisionSlime.Divide();
                }
                else
                {
                    DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.DIE);
                    yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                }
                Destroy(DivisionSlime.gameObject);
            }
        }

        protected override void OnDead()
        {
            ChangeState(new DieState(this));
        }

        public class MoveState : SlimeMoveState { }

        private void Divide()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 3;
                GameObject newSlime = Instantiate(gameObject, transform.position, Quaternion.identity);
                newSlime.transform.localScale = transform.localScale * divideRatio;
                
                newSlime.transform.DOJump(spawnPosition, 1f, 1, 0.5f).SetEase(Ease.OutQuad);

                DivisionSlime slimeComponent = newSlime.GetComponent<DivisionSlime>();
                
                slimeComponent._slimeAnimator = SlimeAnimator.Create(slimeComponent.gameObject, sprites);
                slimeComponent.damage *= divideRatio;
                slimeComponent.MeleeAttackRange *= divideRatio;
                slimeComponent.RangedAttackRange *= divideRatio;
                slimeComponent.airborneRange *= divideRatio;
                slimeComponent.divisionLevel = divisionLevel + 1;
                slimeComponent.StartMob();
            }
        }
    }
}