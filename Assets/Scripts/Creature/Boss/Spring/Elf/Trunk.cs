using System;
using playerCharacter;
using UnityEngine;

namespace Creature.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Trunk : MonoBehaviour
    {
        private float damage = 10f;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake()
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, true);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerCharacter.Instance.Bind(1);
            }
        }
    }
}