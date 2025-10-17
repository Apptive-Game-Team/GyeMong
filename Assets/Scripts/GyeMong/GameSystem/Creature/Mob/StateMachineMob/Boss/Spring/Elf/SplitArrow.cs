using System.Collections;
using System.Collections.Generic;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    public class SplitArrow : ArrowBase
    {
        [SerializeField] private GameObject arrowPrefab; // 분할될 화살 프리팹
        [SerializeField] private float splitAngle = 30f;

        protected override IEnumerator FireArrow(float remainingDistance)
        {
            float halfDistance = remainingDistance / 2f;
            float traveled = 0f;
            rb.velocity = direction * speed;

            while (traveled < halfDistance)
            {
                traveled += speed * Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector2.zero;
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            for (int i = -1; i <= 1; i++)
            {
                float newAngle = baseAngle + i * splitAngle;
                float rad = newAngle * Mathf.Deg2Rad;
                Vector3 newDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

                GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                arrow.GetComponent<ArrowBase>().SetDirection(newDir, targetDistance / 2f);
            }

            Destroy(gameObject);
        }

        protected override IEnumerator OnReachEnd()
        {
            Destroy(gameObject);
            yield return null;
        }
    }
}
