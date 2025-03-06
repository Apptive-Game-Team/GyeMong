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
        public GameObject slimePrefab;
        private int divisionLevel;
        private int maxDivisionLevel;
        private float moveSpeed;
        private Transform target;
        private GameObject rangedAttack;
        private float meleeAttackDelay;
        private float dashAttackDelay;
        private float rangedAttackDelay;
        private float animatorDelay;
        private Animator animator;
        private Coroutine startCoroutine;
        private Coroutine curCoroutine;
        private bool isMove = true;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            animator = GetComponent<Animator>();
            currentHp = 10f;

            slimePrefab = gameObject;

            divisionLevel = 0;
            maxDivisionLevel = 2;
            MeleeAttackRange = 4f;
            RangedAttackRange = 10f;
            moveSpeed = Random.Range(2.5f, 3.5f);

            rangedAttack = transform.GetChild(0).gameObject;

            meleeAttackDelay = 0.3f;
            rangedAttackDelay = 1f;
            dashAttackDelay = 1f;
            animatorDelay= 0.5f;

            damage = 10f;

            enemyAttackInfo = transform.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage / 2, null, false, false, true, meleeAttackDelay);
        }

        private void Start()
        {
            target = PlayerCharacter.Instance.gameObject.transform;
            startCoroutine = StartCoroutine(Patterns());
        }

        private void Update()
        {
            if (target == null) return;
            if (isMove)
            {
                MoveTowardsTarget();
            }
            FaceToPlayer();
        }

        private void MoveTowardsTarget()
        {
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

                yield return new WaitForSeconds(2f);
            }
        }

        private IEnumerator MeleeAttack()
        {
            if (curCoroutine != null) StopCoroutine(curCoroutine);
            isMove = false;
            animator.SetTrigger("MeleeCharge");
            yield return new WaitForSeconds(meleeAttackDelay);

            animator.SetTrigger("MeleeAttack");
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < MeleeAttackRange) PlayerCharacter.Instance.TakeDamage(damage);

            yield return new WaitForSeconds(animatorDelay);
            animator.SetTrigger("Idle");
            yield return new WaitForSeconds(animatorDelay);
            isMove = true;
        }

        private IEnumerator RangedAttack()
        {
            if (curCoroutine != null) StopCoroutine(curCoroutine);
            isMove = false;
            animator.SetTrigger("RangedCharge");
            yield return new WaitForSeconds(rangedAttackDelay);

            animator.SetTrigger("RangedAttack");
            GameObject attack = Instantiate(rangedAttack, transform.position, Quaternion.identity, transform);
            attack.SetActive(true);

            yield return new WaitForSeconds(animatorDelay);
            animator.SetTrigger("Idle");
            yield return new WaitForSeconds(animatorDelay);
            isMove = true;
        }

        private IEnumerator DashAttack()
        {
            if (curCoroutine != null) StopCoroutine(curCoroutine);
            isMove = false;
            animator.SetTrigger("MeleeCharge");

            Vector3 direction = (target.position - transform.position).normalized;
            float dashDistance = 1.5f;
            Vector3 dashTargetPosition = target.position + direction * dashDistance;
            yield return new WaitForSeconds(dashAttackDelay);

            animator.SetTrigger("DashAttack");
            transform.DOMove(dashTargetPosition, 0.3f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(animatorDelay);
            animator.SetTrigger("Idle");
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
                animator.SetTrigger("Dead");
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
                Destroy(newSlime.GetComponent<DivisionSlime>());
                
                newSlime.transform.localScale = transform.localScale * divideRatio;
                
                newSlime.transform.DOMoveY(spawnPosition.y + 1f, 0.3f).SetEase(Ease.OutQuad)
                    .OnComplete(() => newSlime.transform.DOMoveY(spawnPosition.y, 0.3f).SetEase(Ease.InBounce));

                DivisionSlime slimeComponent = newSlime.AddComponent<DivisionSlime>();
                slimeComponent.damage *= damageRatio;
                slimeComponent.MeleeAttackRange *= divideRatio;
                slimeComponent.RangedAttackRange *= divideRatio;
                slimeComponent.divisionLevel = divisionLevel + 1;
            }
        }
    }
}