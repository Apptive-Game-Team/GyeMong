using System;
using System.Collections;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Trunk : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void OnEnable()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            animator.Play("Trunk");
            StartCoroutine(DestroyAfterDelay(0.417f));
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player"))
            {
                SceneContext.Character.Bind(1f);
            }
        }
    }
}