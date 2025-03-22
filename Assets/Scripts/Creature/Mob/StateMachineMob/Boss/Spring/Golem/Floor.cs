using System;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    [Obsolete("Use AttackObjectController instead")]
    public class Floor : MonoBehaviour
    {
        private float damage = 30f;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake()
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, true);
        }
    }
}