using System;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Arrow : MonoBehaviour
    {
        private Vector3 direction;
        private float speed = 15f;
        private Rigidbody2D rb;
        private float targetDistance = 10f;
        private float traveledDistance = 0f;
        private bool isReflected = false;
        private GameObject player;

        private void Awake()
        {
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
            direction = directionToPlayer;
            RotateArrow();
            traveledDistance = 0f;
            rb.velocity = direction * speed;
            while (traveledDistance < remainingDistance)
            {
                traveledDistance += speed * Time.deltaTime;
                yield return null;
            }
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
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
            else if (collision.CompareTag("Boss") && isReflected)
            {
                collision.GetComponent<Boss>().OnAttacked(20f);
                Destroy(gameObject);
            }
        }
    }
}