using UnityEngine;

namespace Creature.Boss
{
    public abstract class BossAttack : MonoBehaviour
    {
        protected GameObject player;
        protected float damage;
        private EnemyAttackInfo enemyAttackInfo;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, false);
        }
    }
}