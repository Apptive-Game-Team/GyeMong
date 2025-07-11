using System;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    [Obsolete("Use AttackObjectController instead")]
    public class ShockWave : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void OnEnable()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            animator.Play("ShockWave");
            StartCoroutine(DestroyAfterDelay(0.5f));
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}