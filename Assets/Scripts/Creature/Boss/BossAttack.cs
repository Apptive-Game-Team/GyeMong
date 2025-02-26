using UnityEngine;

namespace Creature.Boss
{
    public class BossAttack : MonoBehaviour
    {
        protected float damage;
        private EnemyAttackInfo enemyAttackInfo;
        public void SetDamage(float damage)
        {
            this.damage = damage;
        }
        private void Awake()
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, false);
        }
    }
}