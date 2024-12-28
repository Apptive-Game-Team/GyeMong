using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackdamage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        attackdamage = Boss.GetInstance<MidBoss>().defaultDamage;
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(attackdamage);
        }
    }
}
