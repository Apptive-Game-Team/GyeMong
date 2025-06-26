using System;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Trunk : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                SceneContext.Character.Bind(1f);
            }
        }
    }
}