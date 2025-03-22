using System;
using playerCharacter;
using UnityEngine;

namespace Creature.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Trunk : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerCharacter.Instance.Bind(1f);
            }
        }
    }
}