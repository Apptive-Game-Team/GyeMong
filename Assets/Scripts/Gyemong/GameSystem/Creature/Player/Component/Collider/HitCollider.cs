using System.Collections.Generic;
using Gyemong.GameSystem.Creature.Attack;
using Gyemong.GameSystem.Creature.Attack.Component;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Player.Component.Collider
{
    public class HitCollider : MonoBehaviour
    {
        [SerializeField] private AirborneController airborneController;

        private Dictionary<Collider2D, float> multiHitTimers = new Dictionary<Collider2D, float>();

        private void OnTriggerEnter2D(Collider2D other) => HandleAttackCollision(other);
        private void OnTriggerStay2D(Collider2D other) => HandleAttackCollision(other);
        private void OnCollisionEnter2D(Collision2D other) => HandleAttackCollision(other.collider);
        private void OnCollisionStay2D(Collision2D other) => HandleAttackCollision(other.collider);

        private void HandleAttackCollision(Collider2D collider)
        {
            if (!collider.CompareTag("EnemyAttack")) return;

            EnemyAttackInfo enemyAttackInfo = collider.GetComponent<AttackObjectController>()?.AttackInfo;
            if (enemyAttackInfo == null) return;
            AttackObjectController attackObjectController = collider.GetComponent<AttackObjectController>();

            if (enemyAttackInfo.canMultiHit)
            {
                float lastHitTime = multiHitTimers.ContainsKey(collider) ? multiHitTimers[collider] : 0f;

                if (Time.time < lastHitTime + enemyAttackInfo.multiHitDelay) return;

                multiHitTimers[collider] = Time.time;
                PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
            }
            else if (!attackObjectController.isAttacked)
            {
                ApplyAirborne(collider.GetComponent<AttackObjectController>());
                attackObjectController.isAttacked = true;

                if (enemyAttackInfo.soundObject != null)
                    enemyAttackInfo.soundObject.PlayAsync();

                PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
                collider.gameObject.SetActive(!enemyAttackInfo.isDestroyOnHit);
            }
        }

        private void ApplyAirborne(AttackObjectController controller)
        {
            if (controller.AttackInfo.knockbackAmount > 0)
            {
                Vector3 origin = controller.gameObject.transform.position;
                Vector3 direction = (PlayerCharacter.Instance.transform.position - origin).normalized;
                StartCoroutine(airborneController.AirborneTo(direction * controller.AttackInfo.knockbackAmount + PlayerCharacter.Instance.transform.position));
            }
        }
    }
}
