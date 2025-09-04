using System.Collections;
using System.Collections.Generic;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    public class HomingArrow : ArrowBase
    {
        private GameObject player;
        [SerializeField] private float homingDuration = 0.5f;
        [SerializeField] private float rotateSpeed = 720f; // 초당 회전 속도(도)

        private bool homingFinished = false;

        protected override void Awake()
        {
            base.Awake();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        protected override IEnumerator FireArrow(float remainingDistance)
        {
            float elapsed = 0f;
            traveledDistance = 0f;

            while (traveledDistance < remainingDistance)
            {
                if (!homingFinished && elapsed < homingDuration && player != null)
                {
                    // 플레이어 방향으로 유도
                    Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                    float targetAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
                    float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);

                    direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
                    rb.velocity = direction * speed;
                    RotateArrow();
                }
                else if (!homingFinished)
                {
                    // 유도 종료 → 마지막 방향으로 직선 비행 시작
                    homingFinished = true;
                    rb.velocity = direction * speed;
                    RotateArrow();
                }

                traveledDistance += speed * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector2.zero;
            yield return StartCoroutine(OnReachEnd());
        }

        protected override IEnumerator OnReachEnd()
        {
            Destroy(gameObject);
            yield return null;
        }
    }
}