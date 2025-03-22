using System;
using System.Collections;
using UnityEngine;

namespace Creature.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Vine : MonoBehaviour
    {
        private float damage = 20f;
        private float attackDelay = 0.5f;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake()
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, true, true, attackDelay);
        }

        private void OnEnable()
        {
            StartCoroutine(ActivateVine());
        }
        private IEnumerator ActivateVine()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}