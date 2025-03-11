using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using playerCharacter;

namespace Creature.Minion.Slime
{
    public class DivisionSlime : Creature
    {
        private const float divideRatio = 0.6f;
        private const float damageRatio = 0.5f;
        private int divisionLevel;
        private int maxDivisionLevel;
        private float moveSpeed;
        private GameObject rangedAttack;
        private float patternDelay;
        private float dashAttackDelay;
        private float animatorDelay;
        private bool isMove = true;
        private SlimeAnimator _slimeAnimator;
        [SerializeField] private SlimeSprites sprites;

        private bool _isStart = false;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _slimeAnimator = GetComponent<SlimeAnimator>();
            
            maxHp = 10f;

            _isStart = false;
            
            divisionLevel = 0;
            maxDivisionLevel = 2;
            MeleeAttackRange = 4f;
            RangedAttackRange = 10f;
            moveSpeed = Random.Range(2.5f, 3.5f);

            rangedAttack = transform.GetChild(0).gameObject;

            patternDelay = 1f;
            dashAttackDelay = 1f;
            animatorDelay= 0.5f;

            damage = 10f;
        }

        public override void StartMob()
        {
            base.StartMob();
            _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
            _isStart = true;
        }

        public abstract class DivisionSlimeState : BaseState
        {
            public DivisionSlime DivisionSlime => creature as DivisionSlime;
            public override void OnStateUpdate()
            {
                if (!DivisionSlime._isStart) return;
                
                if (PlayerCharacter.Instance.gameObject == null) return;
                if (DivisionSlime.isMove)
                {
                    DivisionSlime.MoveTowardsTarget();
                    DivisionSlime.FaceToPlayer();
                }
            }
        }

        public class MoveState : DivisionSlimeState
        {
            public override int GetWeight()
            {
                return (DivisionSlime.DistanceToPlayer > DivisionSlime.RangedAttackRange) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(DivisionSlime.patternDelay);
                DivisionSlime.ChangeState();
            }
        }

        public class MeleeAttackState : DivisionSlimeState
        {
            public override int GetWeight()
            {
                return (DivisionSlime.DistanceToPlayer < DivisionSlime.MeleeAttackRange) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime.isMove = false;
                
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME * 2);
                
                if (DivisionSlime.DistanceToPlayer < DivisionSlime.MeleeAttackRange) PlayerCharacter.Instance.TakeDamage(DivisionSlime.damage);
                
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
                
                yield return new WaitForSeconds(DivisionSlime.animatorDelay);
                DivisionSlime.isMove = true;

                yield return new WaitForSeconds(DivisionSlime.patternDelay);
                DivisionSlime.ChangeState();
            }
        }

        public class RangedAttackState : DivisionSlimeState
        {
            public override int GetWeight()
            {
                return (DivisionSlime.DistanceToPlayer > DivisionSlime.MeleeAttackRange &&
                DivisionSlime.DistanceToPlayer <= DivisionSlime.RangedAttackRange) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime.isMove = false;
                
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RANGED_ATTACK);
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                
                GameObject attack = Instantiate(DivisionSlime.rangedAttack, DivisionSlime.transform.position, Quaternion.identity, DivisionSlime.transform);
                attack.SetActive(true);
                
                yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
                yield return new WaitForSeconds(DivisionSlime.animatorDelay);
                DivisionSlime.isMove = true;

                yield return new WaitForSeconds(DivisionSlime.patternDelay);
                DivisionSlime.ChangeState();
            }
        }

        public class DashAttackState : DivisionSlimeState
        {
            public override int GetWeight()
            {
                return (DivisionSlime.DistanceToPlayer > DivisionSlime.MeleeAttackRange &&
                DivisionSlime.DistanceToPlayer <= DivisionSlime.RangedAttackRange) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime.isMove = false;
            
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);
                
                float dashDistance = 1.5f;
                Vector3 dashTargetPosition = PlayerCharacter.Instance.transform.position + DivisionSlime.DirectionToPlayer * dashDistance;
                yield return new WaitForSeconds(DivisionSlime.dashAttackDelay);
                
                DivisionSlime.transform.DOMove(dashTargetPosition, 0.3f).SetEase(Ease.OutQuad);

                yield return new WaitForSeconds(DivisionSlime.animatorDelay);
                
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
                yield return new WaitForSeconds(DivisionSlime.animatorDelay);
                DivisionSlime.isMove = true; 

                yield return new WaitForSeconds(DivisionSlime.patternDelay);
                DivisionSlime.ChangeState();
            }
        }


        private void MoveTowardsTarget()
        {
            if (_slimeAnimator.CurrentAnimationType != SlimeAnimator.AnimationType.MELEE_ATTACK)
                _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);
            
            transform.position += DirectionToPlayer * moveSpeed * Time.deltaTime;
        }

        public void FaceToPlayer()
        {
            float scale = Mathf.Abs(transform.localScale.x);
            if (DirectionToPlayer.x < 0)
            {
                transform.localScale = new Vector3(-scale, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
            }
        }

        public override void OnAttacked(float damage)
        {
            base.OnAttacked(damage);
            if (currentHp <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(Die());
            }
        }

        private IEnumerator Die()
        {
            isMove = false;

            if (divisionLevel < maxDivisionLevel)
            {
                Divide();
            }
            else
            {
                _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.DIE);
                yield return new WaitForSeconds(0.3f);
            }
            Destroy(gameObject);
        }

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
                
                slimeComponent.damage *= damageRatio;
                slimeComponent.MeleeAttackRange *= divideRatio;
                slimeComponent.RangedAttackRange *= divideRatio;
                slimeComponent.divisionLevel = divisionLevel + 1;
                slimeComponent.StartMob();
            }
        }
    }
}