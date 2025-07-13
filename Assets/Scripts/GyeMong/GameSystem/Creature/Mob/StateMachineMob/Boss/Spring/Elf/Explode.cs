using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GyeMong.SoundSystem;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    public class Explode : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Animator>().Play("SeedExplode");
            Sound.Play("ENEMY_Seed_Explosion");
            StartCoroutine(DestroyAfterDelay(0.5f));
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}
