using System;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public abstract class ArrowBase : MonoBehaviour
    {
        protected Vector3 direction;
        [SerializeField] protected float speed = 25f;
        protected Rigidbody2D rb;
        protected float targetDistance = 10f;
        protected float traveledDistance = 0f;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public virtual void SetDirection(Vector3 dir, float distance)
        {
            direction = dir.normalized;
            targetDistance = distance;
            RotateArrow();
            StartCoroutine(FireArrow(13f));
        }

        protected void RotateArrow()
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        protected virtual IEnumerator FireArrow(float remainingDistance)
        {
            traveledDistance = 0f;
            rb.velocity = direction * speed;

            while (traveledDistance < remainingDistance)
            {
                traveledDistance += speed * Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector2.zero;
            yield return OnReachEnd();
        }

        protected abstract IEnumerator OnReachEnd();
    }
}