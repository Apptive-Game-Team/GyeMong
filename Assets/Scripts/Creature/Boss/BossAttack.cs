using UnityEngine;

namespace Rework
{
    public class BossAttack : MonoBehaviour
    {
        protected float damage;
        public void SetDamage(float damage)
        {
            this.damage = damage;
        }
    }
}