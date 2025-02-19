using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            CheckAttaching(other);
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
            CheckAttaching(other.collider);
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
            playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
            Destroy(other.gameObject);
        }
        else
        {
            playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
        }
    }

    private void CheckAttaching(Collider2D other)
    {
        EnemyAttackInfo enemyAttackInfo = other.GetComponent<EnemyAttackInfo>();
        if (enemyAttackInfo == null) return;

        if (enemyAttackInfo.isMultiHit)
        {
            playerCharacter.PlayerCharacter.Instance.TakeDamage(enemyAttackInfo.damage);
            StartCoroutine(Wait(enemyAttackInfo.multiHitDelay));
        }
    }

    private IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
