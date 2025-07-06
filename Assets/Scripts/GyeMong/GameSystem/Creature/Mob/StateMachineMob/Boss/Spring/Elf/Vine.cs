using System;
using System.Collections;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Vine : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void OnEnable()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            animator.Play("Vine");
            StartCoroutine(DestroyAfterDelay(2f));
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}