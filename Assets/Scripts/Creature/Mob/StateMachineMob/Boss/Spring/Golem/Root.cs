using System;
using System.Collections;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    [Obsolete("Use AttackObjectController instead")]
    public class Root : MonoBehaviour
    {
        private float damage = 30f;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake()
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, false);
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
    }
}