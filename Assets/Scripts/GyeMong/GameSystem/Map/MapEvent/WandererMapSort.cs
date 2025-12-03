using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.MapEvent
{
    public class WandererMapSort : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer PlayerSr;
        [SerializeField] private SpriteRenderer WandererSwordSr;
        [SerializeField] private SpriteRenderer WandererSr;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) PlayerSr.sortingOrder = 14;
            if (other.CompareTag("Boss"))
            {
                WandererSr.sortingOrder = 14;
                WandererSwordSr.sortingOrder = 14;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) PlayerSr.sortingOrder = 12;
            if (other.CompareTag("Boss"))
            {
                WandererSr.sortingOrder = 12;
                WandererSwordSr.sortingOrder = 3;
            }
        }
    }
}