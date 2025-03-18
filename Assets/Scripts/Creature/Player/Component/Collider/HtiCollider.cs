using System.Collections;
using System.Game;
using Creature.Attack;
using Creature.Attack.Component;
using UnityEngine;


namespace Creature.Player.Component.Collider
{
    public class HitCollider : MonoBehaviour
    {
        [SerializeField] private AirborneController airborneController;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("EnemyAttack"))
            {
                EnemyAttackInfo enemyAttackInfo = other.GetComponent<AttackObjectController>().AttackInfo;
                AttackObjectController attackObjectController = other.GetComponent<AttackObjectController>();
                if (enemyAttackInfo == null) return;

                ApplyAirborne(attackObjectController);
                attackObjectController.isAttacked = true;
                if (enemyAttackInfo.soundObject != null)
                {
                    enemyAttackInfo.soundObject.PlayAsync();
                }

                if (enemyAttackInfo.isDestroyOnHit)
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                    other.gameObject.SetActive(false);
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
                EnemyAttackInfo enemyAttackInfo = other.GetComponent<AttackObjectController>().AttackInfo;
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
                EnemyAttackInfo enemyAttackInfo = other.collider.GetComponent<AttackObjectController>().AttackInfo;
                if (enemyAttackInfo == null) return;

                other.collider.GetComponent<AttackObjectController>().isAttacked = true;
                if (enemyAttackInfo.soundObject != null)
                {
                    enemyAttackInfo.soundObject.PlayAsync();
                }

                if (enemyAttackInfo.isDestroyOnHit)
                {
                    playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                    other.collider.gameObject.SetActive(false);
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
                EnemyAttackInfo enemyAttackInfo = other.collider.GetComponent<AttackObjectController>().AttackInfo;
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

        private void ApplyAirborne(AttackObjectController controller)
        {
            if (controller.AttackInfo.knockbackAmount > 0)
            {
                Vector3 origin = controller.gameObject.transform.position;
                Vector3 direction = playerCharacter.PlayerCharacter.Instance.transform.position - origin;
                StartCoroutine(airborneController.AirborneTo(direction * controller.AttackInfo.knockbackAmount + playerCharacter.PlayerCharacter.Instance.transform.position));
            }
        }
    }
}
