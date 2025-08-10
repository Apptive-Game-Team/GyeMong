using System;
using System.Collections;
using GyeMong.SoundSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Seed : MonoBehaviour
    {
        private GameObject player;
        private Vector3 direction;
        private float speed = 30f;
        private SoundObject _explosionSoundObject;
        private Rigidbody2D rb;
        private float targetDistance = 10f;
        private float traveledDistance = 0f;
        private bool isReflected = false;
        private float angleRange = 20f;
        [SerializeField] private GameObject explodePrefab;

        private void Awake()
        {
            _explosionSoundObject = GetComponent<SoundObject>();
            player = GameObject.FindGameObjectWithTag("Player");
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
            while (traveledDistance < remainingDistance)
            {
                traveledDistance += speed * Time.deltaTime;
                yield return null;
            }
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(1f);
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            _explosionSoundObject.PlayAsync();
            GameObject obj = Instantiate(explodePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            yield return null;
        }

        /*private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerAttack") && !isReflected)
            {
                isReflected = true;
                Vector2 playerAttackDirection = SceneContext.Character.mouseDirection;
                direction = playerAttackDirection.normalized;
                traveledDistance = 0f;
                rb.velocity = direction * speed;
                RotateArrow();
            }
            /*else if (collision.CompareTag("Boss") && isReflected)
            {
                collision.GetComponent<Elf>().StartCoroutine(collision.GetComponent<Elf>().Stun(5f));
                Destroy(gameObject);
            }#1#
        }*/
    }
}
