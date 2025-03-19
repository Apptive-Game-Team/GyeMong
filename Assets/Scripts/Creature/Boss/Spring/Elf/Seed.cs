using System;
using playerCharacter;
using System.Collections;
using System.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creature.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Seed : BossAttack
    {
        private Vector3 direction;
        private float speed = 15f;
        private SoundObject _explosionSoundObject;
        private EventObject _eventObject;
        private Rigidbody2D rb;
        private float targetDistance = 10f;
        private float traveledDistance = 0f;
        private bool isReflected = false;
        private float angleRange = 20f;
        private float explosionRadius = 2f;
         
        protected override void Awake()
        {
            base.Awake();
            damage = 20f;
            _eventObject = GetComponent<EventObject>();
            _soundObject = GameObject.Find("ArrowHitSoundObject").GetComponent<SoundObject>();
            _explosionSoundObject = GetComponent<SoundObject>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            StartCoroutine(FireArrow(targetDistance));
            RotateArrow();
        }

        private void RotateArrow()
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private IEnumerator FireArrow(float remainingDistance)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            float randomAngle = Random.Range(baseAngle - angleRange, baseAngle + angleRange);

            float randomAngleRad = randomAngle * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad), 0).normalized;

            transform.rotation = Quaternion.Euler(0, 0, randomAngle);
            traveledDistance = 0f;
            rb.velocity = direction * speed;
            _soundObject.PlayAsync();
            while (traveledDistance < remainingDistance)
            {
                traveledDistance += speed * Time.deltaTime;
                yield return null;
            }
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(1f);
            Explode();
        }

        private void Explode()
        {
            _explosionSoundObject.PlayAsync();
            _eventObject.Trigger();
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    PlayerCharacter.Instance.TakeDamage(damage / 2);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerAttack") && !isReflected)
            {
                isReflected = true;
                Vector2 playerAttackDirection = PlayerCharacter.Instance.mouseDirection;
                direction = playerAttackDirection.normalized;
                traveledDistance = 0f;
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
