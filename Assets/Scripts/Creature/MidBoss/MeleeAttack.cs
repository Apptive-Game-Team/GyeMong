using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : GrazeController
{
    public float attackdamage;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        attackdamage = Boss.GetInstance<MidBoss>().defaultDamage;
        if (other.CompareTag("Player"))
        {
            isAttacked = true;
            PlayerCharacter.Instance.TakeDamage(attackdamage);
        }
    }
}
