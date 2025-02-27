using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Spring.Elf
{
    public class Seed : MonoBehaviour
    {
        private GameObject player;
        private Vector3 direction;
        private float speed = 15f;
        private float damage = 20f;
        private EnemyAttackInfo enemyAttackInfo;

        private SoundObject _soundObject;
        private SoundObject _explosionSoundObject;
        private EventObject _eventObject;
        private Rigidbody2D rb;
        private bool isReflected = false;

        private void Awake()
        {
            _eventObject = GetComponent<EventObject>();
            _soundObject = GameObject.Find("ArrowHitSoundObject").GetComponent<SoundObject>();
            _explosionSoundObject = GetComponent<SoundObject>();
            player = GameObject.FindGameObjectWithTag("Player");
            rb = GetComponent<Rigidbody2D>();

            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, _soundObject, true, true);
        }

        private void OnEnable()
        {
            StartCoroutine(FireArrow());
            RotateArrow();
        }

        private void RotateArrow()
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private IEnumerator FireArrow()
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            float angleRange = 20f;
            float randomAngle = Random.Range(baseAngle - angleRange, baseAngle + angleRange);

            float randomAngleRad = randomAngle * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad), 0).normalized;

            transform.rotation = Quaternion.Euler(0, 0, randomAngle);
            rb.velocity = direction * speed;
            yield return new WaitForSeconds(5f);
            Explode();
        }

        private void Explode()
        {
            _explosionSoundObject.PlayAsync();
            _eventObject.Trigger();
            float explosionRadius = 2f;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    PlayerCharacter.Instance.TakeDamage(damage / 2);
                }
            }
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerAttack") && !isReflected)
            {
                isReflected = true;
                direction = -direction;
                rb.velocity = direction * speed;
                RotateArrow();
            }
            else if (collision.CompareTag("Boss") && isReflected)
            {
                collision.GetComponent<Boss>().StartCoroutine(collision.GetComponent<Boss>().Stun());
                Destroy(gameObject);
            }
        }
    }
}
