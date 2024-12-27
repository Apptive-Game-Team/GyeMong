using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private float attackdamage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        attackdamage = MidBoss.Instance.defaultDamage;
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(attackdamage);
        }
    }
}
