using System;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Arrow : MonoBehaviour
    {
        private Vector3 direction;
        private float speed = 30f;
        private Rigidbody2D rb;
        private float targetDistance = 10f;
        private float traveledDistance = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public void SetDirection(Vector3 dir, float distance)
        {
            direction = dir.normalized;
            targetDistance = distance;
            RotateArrow();
            StartCoroutine(FireArrow(targetDistance));
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
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}