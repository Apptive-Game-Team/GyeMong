using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using playerCharacter;
using Creature;

namespace Creature.Minion.Slime
{
    public class DivisionSlime : Creature
    {
        public GameObject slimePrefab;
        [SerializeField] public int divisionLevel;
        [SerializeField] private int maxDivisionLevel;
        [SerializeField] private float attackRange;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Transform target;
        private const float divideRatio = 0.6f;

        private void Awake()
        {
            currentHp = 10f;
            slimePrefab = gameObject;
            divisionLevel = 0;
            maxDivisionLevel = 2;
            attackRange = 2f;
            moveSpeed = Random.Range(2.5f, 3.5f);
        }

        private void Start()
        {
            target = PlayerCharacter.Instance.gameObject.transform;
            StartCoroutine(AttackPattern());
        }

        private void Update()
        {
            if (target == null) return;
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        private IEnumerator AttackPattern()
        {
            while (true)
            {
                if (target == null)
                {
                    yield return null;
                    continue;
                }

                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange)
                {
                    MeleeAttack();
                }
                else
                {
                    if (Random.value < 0.5f)
                    {
                        RangedAttack();
                    }
                    else
                    {
                        DashAttack();
                    }
                }

                yield return new WaitForSeconds(2f);
            }
        }

        private void MeleeAttack()
        {

        }

        private void RangedAttack()
        {

        }

        private void DashAttack()
        {
            
        }

        public override void OnAttacked(float damage)
        {
            base.OnAttacked(damage);
            if (currentHp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (divisionLevel < maxDivisionLevel)
            {
                Divide();
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
                slimeComponent.divisionLevel = divisionLevel + 1;
            }
        }
    }
}
