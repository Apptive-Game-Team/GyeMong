using System.Sound;
using UnityEngine;

namespace Creature.Boss
{
    public abstract class BossAttack : MonoBehaviour
    {
        protected GameObject player;
        protected float damage;
        protected EnemyAttackInfo enemyAttackInfo;
        protected SoundObject _soundObject;
        protected virtual void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, _soundObject, false, false);
        }
    }
}