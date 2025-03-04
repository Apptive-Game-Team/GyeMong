using playerCharacter;
using System.Collections;
using UnityEngine;
using Creature.Boss;

namespace Creature.Minion.Slime
{
    public class RangedAttack : MonoBehaviour
    {
        private GameObject player;
        private Vector3 direction;
        private float speed = 15f;
        private SoundObject _soundObject;
        private float damage = 20f;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake()
        {
            player = PlayerCharacter.Instance.gameObject;
            direction = (player.transform.position - transform.position).normalized;

            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, true, true);
        }

        private void OnEnable()
        {
            StartCoroutine(FireArrow());
            RotateArrow();
        }
        private void RotateArrow()
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        }
        private IEnumerator FireArrow()
        {
            Vector3 firePosition = transform.position;
            float fireDistance = 0;
            while (fireDistance < 10f)
            {
                transform.position += direction * speed * Time.deltaTime;
                fireDistance = Vector3.Distance(firePosition, transform.position);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}