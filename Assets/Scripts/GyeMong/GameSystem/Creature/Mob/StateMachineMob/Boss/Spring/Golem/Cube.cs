using System;
using System.Collections;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    [Obsolete("Use AttackObjectController instead")]
    public class Cube : MonoBehaviour
    {
        private GameObject player;
        [SerializeField] private GameObject cubeShadowPrefab;
        private GameObject cubeShadow;
        private CubeShadow cubeShadowComp;
        private bool isFalled = false;

        [SerializeField] private float riseDuration = 0.2f;
        [SerializeField] private float preFollowDelay = 2f;
        [SerializeField] private float followDuration = 1f;

        private void OnEnable()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(RisingAndFollow());
        }

        private IEnumerator RisingAndFollow()
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * 30f;

            float t = 0f;
            Vector3 originalScale = transform.localScale;
            Quaternion originalRot = transform.rotation;
            float randomAngle = UnityEngine.Random.Range(-5f, 5f);

            while (t < riseDuration)
            {
                float u = t / riseDuration;
                float eased = Mathf.SmoothStep(0f, 1f, u);

                transform.position = Vector3.LerpUnclamped(startPos, endPos, eased);

                float scalePunch = 1f + Mathf.Sin(u * Mathf.PI) * 0.08f;
                transform.localScale = originalScale * scalePunch;

                float rotationOffset = Mathf.Sin(u * Mathf.PI) * randomAngle;
                transform.rotation = Quaternion.Euler(0f, 0f, rotationOffset);

                t += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
            transform.localScale = originalScale;
            transform.rotation = originalRot;

            yield return new WaitForSeconds(2f);
            SpawnShadowAtFollowStart();
            StartCoroutine (FollowAndFall());
        }
        private void SpawnShadowAtFollowStart()
        {
            if (cubeShadowPrefab == null) return;

            cubeShadow = Instantiate(
                cubeShadowPrefab,
                (player != null ? player.transform.position : transform.position) + new Vector3(0, -0.6f, 0),
                Quaternion.identity
            );

            cubeShadowComp = cubeShadow.GetComponent<CubeShadow>();
            if (cubeShadowComp != null)
            {
                // �÷��̾� �߹��� ���󰡸� alpha�� 0��1�� ���̵���
                cubeShadowComp.Initialize(
                    playerTransform: player != null ? player.transform : null,
                    followOffset: new Vector3(0, -0.6f, 0),
                    fadeInTime: 0.35f,          // ���ϴ� ������ ����
                    startAlpha: 0.0f,           // ���� ���� �� ���̰�
                    endAlpha: 1.0f
                );
            }
        }
        private IEnumerator FollowAndFall()
        {
            float followDuration = 1f;
            float elapsedTime = 0f;
            while (elapsedTime < followDuration)
            {
                if (player != null)
                {
                    transform.position = player.transform.position + new Vector3(0, 30, 0);
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Sound.Play("ENEMY_Toss");
            cubeShadowComp.LockAt();
            StartCoroutine(StartFalling());
        }

        private IEnumerator StartFalling()
        {
            float accele = 50f;
            float speed = 50f;
            float currentSpeed = speed;
            Vector3 targetPosition = player.transform.position + new Vector3(0,0.6f,0);
            while (transform.position.y > targetPosition.y)
            {
                currentSpeed += accele * Time.deltaTime;
                float newY = transform.position.y - currentSpeed * Time.deltaTime;
                newY = Mathf.Max(newY, targetPosition.y);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);

                yield return null;
            }
            isFalled = true;
            Sound.Play("ENEMY_Rock_Falled");
            SceneContext.CameraManager.CameraShake(0.15f);
            SceneContext.CameraManager.CameraShake(0.1f);
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
            yield return new WaitForSeconds(0.5f);

            Destroy(gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (isFalled && collision.collider.CompareTag("Boss"))
            {
                Debug.Log("Check0");
                Golem golem = collision.collider.GetComponent<Golem>();
                if (golem != null)
                {
                    golem.StartCoroutine(golem.Stun(1f));
                }
                Destroy(gameObject);
            }
        }
        private void OnDestroy()
        {
            if (cubeShadow != null)
            {
                Destroy(cubeShadow);
            }
        }
    }
}