using System.Collections;
using UnityEngine;
using DG.Tweening;
using playerCharacter;
using Creature.Boss;
using Unity.VisualScripting;

namespace Creature.Minion.Slime
{
    public class DivisionSlime : Creature
    {
        private const float divideRatio = 0.6f;
        private const float damageRatio = 0.5f;
        private int divisionLevel;
        private int maxDivisionLevel;
        private float moveSpeed;
        private Transform target;
        private GameObject rangedAttack;
        private float patternDelay;
        private float meleeAttackDelay;
        private float dashAttackDelay;
        private float rangedAttackDelay;
        private float animatorDelay;
        private Animator animator;
        private Coroutine startCoroutine;
        private Coroutine curCoroutine;
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
            
            currentHp = 10f;

            _isStart = false;
            
            divisionLevel = 0;
            maxDivisionLevel = 2;
            MeleeAttackRange = 4f;
            RangedAttackRange = 10f;
            moveSpeed = Random.Range(2.5f, 3.5f);

            rangedAttack = transform.GetChild(0).gameObject;

            patternDelay = 2f;
            meleeAttackDelay = 0.3f;
            rangedAttackDelay = 1f;
            dashAttackDelay = 1f;
            animatorDelay= 0.5f;

            damage = 10f;
        }

        public void StartMob()
        {
            target = PlayerCharacter.Instance.gameObject.transform;
            startCoroutine = StartCoroutine(Patterns());
            _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
            _isStart = true;
        }

        private void Update()
        {
            if (!_isStart) return;
            
            if (target == null) return;
            if (isMove)
            {
                MoveTowardsTarget();
            }
            FaceToPlayer();
        }

        private void MoveTowardsTarget()
        {
            if (_slimeAnimator.CurrentAnimationType != SlimeAnimator.AnimationType.MELEE_ATTACK)
                _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);
            
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        public void FaceToPlayer()
        {
            float scale = Mathf.Abs(transform.localScale.x);
            if (PlayerCharacter.Instance.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-scale, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
            }
        }

        private IEnumerator Patterns()
        {
            while (true)
            {
                if (target == null)
                {
                    yield return null;
                    continue;
                }

                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= MeleeAttackRange)
                {
                    curCoroutine = StartCoroutine(MeleeAttack());
                }
                else
                {
                    if (distance <= RangedAttackRange)
                    {
                        if (Random.value < 0.5f)
                        {
                            curCoroutine = StartCoroutine(RangedAttack());
                        }
                        else
                        {
                            curCoroutine = StartCoroutine(DashAttack());
                        }
                    }
                }

                yield return new WaitForSeconds(patternDelay);
            }
        }

        private IEnumerator MeleeAttack()
        {
            if (curCoroutine != null) StopCoroutine(curCoroutine);
            isMove = false;
            
            _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK);
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME * 2);
            
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < MeleeAttackRange) PlayerCharacter.Instance.TakeDamage(damage);
            
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
            _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
            
            yield return new WaitForSeconds(animatorDelay);
            isMove = true;
        }

        private IEnumerator RangedAttack()
        {
            if (curCoroutine != null) StopCoroutine(curCoroutine);
            isMove = false;
            
            _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RANGED_ATTACK);
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
            
            GameObject attack = Instantiate(rangedAttack, transform.position, Quaternion.identity, transform);
            attack.SetActive(true);
            
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
            _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
            yield return new WaitForSeconds(animatorDelay);
            isMove = true;
        }

        private IEnumerator DashAttack()
        {
            if (curCoroutine != null) StopCoroutine(curCoroutine);
            isMove = false;
            
            _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);

            Vector3 direction = (target.position - transform.position).normalized;
            float dashDistance = 1.5f;
            Vector3 dashTargetPosition = target.position + direction * dashDistance;
            yield return new WaitForSeconds(dashAttackDelay);
            
            transform.DOMove(dashTargetPosition, 0.3f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(animatorDelay);
            
            _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
            yield return new WaitForSeconds(animatorDelay);
            isMove = true; 
        }

        public override void OnAttacked(float damage)
        {
            base.OnAttacked(damage);
            if (currentHp <= 0)
            {
                StartCoroutine(Die());
            }
        }

        private IEnumerator Die()
        {
            isMove = false;
            StopCoroutine(startCoroutine);

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
                // Destroy(newSlime.GetComponent<DivisionSlime>());
                
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