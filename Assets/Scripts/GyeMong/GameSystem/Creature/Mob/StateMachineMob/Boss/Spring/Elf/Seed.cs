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
        private Vector3 direction;
        private float speed = 30f;
        private Rigidbody2D rb;

        [SerializeField] private GameObject explodePrefab;
        private SoundObject _explosionSoundObject;

        private float targetDistance = 10f;
        private float traveledDistance = 0f;

        private void Awake()
        {
            _explosionSoundObject = GetComponent<SoundObject>();
            rb = GetComponent<Rigidbody2D>();
        }
        public void SetDirection(Vector3 dir, float distance)
        {
            direction = dir.normalized;
            targetDistance = distance;
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
            Instantiate(explodePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(Explode());
            }
        }
    }
}
