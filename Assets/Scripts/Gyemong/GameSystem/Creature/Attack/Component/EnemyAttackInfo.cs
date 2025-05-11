using Gyemong.SoundSystem;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Attack.Component
{
    [CreateAssetMenu(fileName = "EnemyAttackInfo", menuName = "Creature/Attack/EnemyAttackInfo")]
    public class EnemyAttackInfo : ScriptableObject
    {
        public float damage = 0f;
        public SoundObject soundObject = null;
        public bool isDestroyOnHit = false;
        public bool grazable = false;
        public bool canMultiHit = false;
        public float multiHitDelay = 0f;
        public float knockbackAmount = 0f;
    }
}