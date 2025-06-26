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
        private bool isFalled = false;
        private void OnEnable()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FollowAndFall());
            cubeShadow = Instantiate(cubeShadowPrefab, SceneContext.Character.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
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
            StartCoroutine(StartFalling());
        }

        private IEnumerator StartFalling()
        {
            float accele = 70f;
            float speed = 70f;
            float currentSpeed = speed;
            Vector3 targetPosition = player.transform.position;
            while (transform.position.y > targetPosition.y)
            {
                currentSpeed += accele * Time.deltaTime;
                float newY = transform.position.y - currentSpeed * Time.deltaTime;
                newY = Mathf.Max(newY, targetPosition.y);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);

                yield return null;
            }
            Sound.Play("ENEMY_Rock_Falled");
            SceneContext.CameraManager.CameraShake(0.1f);
            Collider2D collider = GetComponent<Collider2D>();
            isFalled = true;
            if (collider != null)
            {
                collider.isTrigger = false;
            }
            yield return new WaitForSeconds(1f);

            Destroy(gameObject);
            Destroy(cubeShadow);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (isFalled && other.CompareTag("Boss"))
            {
                Debug.Log("Check0");
                other.GetComponent<Golem>().StartCoroutine(other.GetComponent<Golem>().Stun(5f));
                Destroy(gameObject);
                Destroy(cubeShadow);
            }
        }
    }
}