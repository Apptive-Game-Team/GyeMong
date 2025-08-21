using System;
using System.Collections;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.SoundSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    public class BindingArrow : ArrowBase
    {
        [SerializeField] private GameObject trunkPrefab;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override IEnumerator OnReachEnd()
        {
            AttackObjectController.Create(
                    transform.position,
                    Vector3.zero,
                    trunkPrefab,
                    new StaticMovement(
                        transform.position,
                        2f)
                )
                .StartRoutine();
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