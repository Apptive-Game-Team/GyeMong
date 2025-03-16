using System.Collections;
using System.Game;
using UnityEngine;
using Creature.Boss;

namespace Creature.Player.Component.Collider
{
    public class HitCollider : MonoBehaviour
    {
        [SerializeField] private AirborneController airborneController;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("EnemyAttack"))
            {
                EnemyAttackInfo enemyAttackInfo = other.GetComponent<EnemyAttackInfo>();
                if (enemyAttackInfo == null) return;

                ApplyAirborne(enemyAttackInfo);
                enemyAttackInfo.isAttacked = true;
                if (enemyAttackInfo.soundObject != null)
                {
                    enemyAttackInfo.soundObject.PlayAsync();
                }

                if (enemyAttackInfo.isDestroyOnHit)
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                    Destroy(other.gameObject);
                }
                else
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("EnemyAttack"))
            {
                EnemyAttackInfo enemyAttackInfo = other.GetComponent<EnemyAttackInfo>();
                if (enemyAttackInfo == null) return;

                if (enemyAttackInfo.isMultiHit)
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                    StartCoroutine(Wait(enemyAttackInfo.multiHitDelay));
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("EnemyAttack"))
            {
                EnemyAttackInfo enemyAttackInfo = other.collider.GetComponent<EnemyAttackInfo>();
                if (enemyAttackInfo == null) return;

                enemyAttackInfo.isAttacked = true;
                if (enemyAttackInfo.soundObject != null)
                {
                    enemyAttackInfo.soundObject.PlayAsync();
                }

                if (enemyAttackInfo.isDestroyOnHit)
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                    Destroy(other.collider.gameObject);
                }
                else
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                }
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.CompareTag("EnemyAttack"))
            {
                EnemyAttackInfo enemyAttackInfo = other.collider.GetComponent<EnemyAttackInfo>();
                if (enemyAttackInfo == null) return;

                if (enemyAttackInfo.isMultiHit)
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                    StartCoroutine(Wait(enemyAttackInfo.multiHitDelay));
                }
            }
        }

        private IEnumerator Wait(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        private void ApplyAirborne(EnemyAttackInfo enemyAttackInfo)
        {
            if (enemyAttackInfo.knockbackAmount > 0)
            {
                Vector3 origin = enemyAttackInfo.gameObject.transform.position;
                Vector3 direction = playerCharacter.PlayerCharacter.Instance.transform.position - origin;
                StartCoroutine(airborneController.AirborneTo(direction * enemyAttackInfo.knockbackAmount + playerCharacter.PlayerCharacter.Instance.transform.position));
            }
        }
    }
}
