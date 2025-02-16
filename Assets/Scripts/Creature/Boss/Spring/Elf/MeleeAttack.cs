using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using Rework;
using UnityEngine;

public class MeleeAttack : BossAttack
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        damage = 20;
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
        }
    }
}
