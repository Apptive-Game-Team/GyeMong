using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
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
}
