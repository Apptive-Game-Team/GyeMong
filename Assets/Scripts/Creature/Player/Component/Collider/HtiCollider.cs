using System.Collections;
using UnityEngine;
using playerCharacter;

namespace Creature.Player.Component.Collider
{
    public class HitCollider : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("EnemyAttack"))
            {
                CheckAttacked(other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("EnemyAttack"))
            {
                CheckAttached(other);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("EnemyAttack"))
            {
                CheckAttacked(other.collider);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.CompareTag("EnemyAttack"))
            {
                CheckAttached(other.collider);
            }
        }

        private void CheckAttacked(Collider2D other)
        {
            EnemyAttackInfo enemyAttackInfo = other.GetComponent<EnemyAttackInfo>();
            if (enemyAttackInfo == null) return;

            enemyAttackInfo.isAttacked = true;
            if (enemyAttackInfo.soundObject != null)
            {
                enemyAttackInfo.soundObject.PlayAsync();
            }

            if (enemyAttackInfo.isDestroyOnHit)
            {
                TakeDamage(enemyAttackInfo.damage);
                Destroy(other.gameObject);
            }
            else
            {
                TakeDamage(enemyAttackInfo.damage);
            }
        }
        
        private void CheckAttached(Collider2D other)
        {
            EnemyAttackInfo enemyAttackInfo = other.GetComponent<EnemyAttackInfo>();
            if (enemyAttackInfo == null) return;

            if (enemyAttackInfo.isMultiHit)
            {
                TakeDamage(enemyAttackInfo.damage);
                StartCoroutine(Wait(enemyAttackInfo.multiHitDelay));
            }
        }


        private IEnumerator Wait(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        public void TakeDamage(float damage)
        {
            PlayerCharacter player = PlayerCharacter.Instance;

            if (player.IsInvincible) return;
            
            StartCoroutine(EffectManager.Instance.ShakeCamera());

            player.CurHealth -= damage;
            player.changeListenerCaller.CallHpChangeListeners(player.CurHealth);
            TakeGauge();
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - player.CurHealth / player.MaxHealth));
            
            if (player.CurHealth <= 0)
            {
                StartCoroutine(player.TriggerInvincibility());
                player.Die();
            }
            else
            {
                StartCoroutine(player.TriggerInvincibility());
            }
        }

        private void TakeGauge()
        {
            PlayerCharacter player = PlayerCharacter.Instance;

            player.CurSkillGauge -= player.GaugeIncreaseValue;
            if (player.CurSkillGauge < 0)
            {
                player.CurSkillGauge = 0f;
            }
            player.changeListenerCaller.CallSkillGaugeChangeListeners(player.CurSkillGauge);
        }
    }
}