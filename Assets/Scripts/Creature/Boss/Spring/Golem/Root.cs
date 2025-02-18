using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using Rework;
using UnityEngine;

public class Root : MonoBehaviour
{
    private float damage = 30f;
    private EnemyAttackInfo enemyAttackInfo;

    private void Awake()
    {
        enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
        enemyAttackInfo.Initialize(damage, null, false, false, false, false);
    }
    private void OnEnable()
    {
        StartCoroutine(OffRootObjects());
    }
    private IEnumerator OffRootObjects()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        damage = 10;
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
        }
    }*/
}
