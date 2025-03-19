using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Creature.Boss
{
    public class EnemyAttackInfo : MonoBehaviour
    {
        public float damage;
        public SoundObject soundObject;
        public bool isDestroyOnHit;
        public bool grazable;
        public bool grazed;
        public bool isAttacked;
        public bool isMultiHit;
        public float multiHitDelay;
        public float knockbackAmount;

        public void Initialize(float damage = 0f, SoundObject soundObject = null, bool isDestroyOnHit = false, bool grazable = false,
            bool isMultiHit = false, float multiHitDelay = 0f, bool grazed = false, bool isAttacked = false, float knockbackAmount = 0f)
        {
            this.damage = damage;
            this.soundObject = soundObject;
            this.isDestroyOnHit = isDestroyOnHit;
            this.grazable = grazable;
            this.grazed = grazed;
            this.isAttacked = isAttacked;
            this.isMultiHit = isMultiHit;
            this.multiHitDelay = multiHitDelay;
        }
    }
}