using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Spring.Golem
{
    public class Cube : MonoBehaviour
    {
        private GameObject player;
        private float damage = 30f;
        private EnemyAttackInfo enemyAttackInfo;

        private bool isFalled = false;

        [SerializeField] private SoundObject _soundObject;

        private void Awake()
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, _soundObject, false, true);
        }

        private void OnEnable()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FollowAndFall());
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

            StartCoroutine(StartFalling());
        }

        private IEnumerator StartFalling()
        {
            float accele = 70f;
            float speed = 70f;
            float currentSpeed = speed;
            Vector3 targetPosition = player.transform.position;
            Vector3 startPosition = transform.position;

            while (transform.position.y > targetPosition.y)
            {
                currentSpeed += accele * Time.deltaTime;
                float newY = transform.position.y - currentSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);

                yield return null;
            }
            Collider2D collider = GetComponent<Collider2D>();
            isFalled = true;
            StartCoroutine(_soundObject.Play());
            if (collider != null)
            {
                collider.isTrigger = false;
            }
            yield return new WaitForSeconds(1f);

            Destroy(gameObject);
            Destroy(shadow);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (isFalled && other.CompareTag("Boss"))
            {
                other.GetComponent<Boss>().StartCoroutine(other.GetComponent<Boss>().Stun(5f));
                Destroy(gameObject);
                Destroy(shadow);
            }
        }
        GameObject shadow;
        public void DetectShadow(GameObject shadow)
        {
            this.shadow = shadow;
        }
    }
}