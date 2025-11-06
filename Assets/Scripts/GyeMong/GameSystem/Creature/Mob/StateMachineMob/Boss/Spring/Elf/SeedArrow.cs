using System;
using System.Collections;
using GyeMong.SoundSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class SeedArrow : ArrowBase
    {
        [SerializeField] private GameObject explodePrefab;
        private SoundObject _explosionSoundObject;

        protected override void Awake()
        {
            base.Awake();
            _explosionSoundObject = GetComponent<SoundObject>();
        }

        protected override IEnumerator OnReachEnd()
        {
            _explosionSoundObject.PlayAsync();
            Instantiate(explodePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(OnReachEnd());
            }
        }
    }
}
