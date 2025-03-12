using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using playerCharacter;

namespace Creature.Minion.Slime
{
    public class DivisionSlime : SlimeBase
    {
        private const float divideRatio = 0.6f;
        private const float damageRatio = 0.5f;
        private int divisionLevel = 0;
        private int maxDivisionLevel = 2;

        
        protected override void Start()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            maxHp = 10f;
            currentHp = maxHp;
            
            MeleeAttackRange = 4f;
            RangedAttackRange = 10f;

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
        }

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
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
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

        protected override SlimeDieState CreateDieState()
        {
            return new DieState(this);
        }

        public class MoveState : SlimeMoveState { }

        private void Divide()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere;
                GameObject newSlime = Instantiate(gameObject, spawnPosition, Quaternion.identity);
                newSlime.transform.localScale = transform.localScale * divideRatio;
                
                newSlime.transform.DOMoveY(spawnPosition.y + 1f, 0.3f).SetEase(Ease.OutQuad)
                    .OnComplete(() => newSlime.transform.DOMoveY(spawnPosition.y, 0.3f).SetEase(Ease.InBounce));

                DivisionSlime slimeComponent = newSlime.GetComponent<DivisionSlime>();
                
                slimeComponent._slimeAnimator = SlimeAnimator.Create(slimeComponent.gameObject, sprites);
                slimeComponent.damage *= damageRatio;
                slimeComponent.MeleeAttackRange *= divideRatio;
                slimeComponent.RangedAttackRange *= divideRatio;
                slimeComponent.divisionLevel = divisionLevel + 1;
                slimeComponent.StartMob();
            }
        }
    }
}