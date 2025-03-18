using UnityEngine;

namespace Creature.Attack.Component
{
    [CreateAssetMenu(fileName = "EnemyAttackInfo", menuName = "Creature/Attack/EnemyAttackInfo")]
    public class EnemyAttackInfo : ScriptableObject
    {
        public float damage = 0f;
        public SoundObject soundObject = null;
        public bool isDestroyOnHit = false;
        public bool grazable = false;
        public bool isMultiHit = false;
        public float multiHitDelay = 0f;
        public float knockbackAmount = 0f;
    }
}