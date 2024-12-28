using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float? attackdamage = MidBoss.GetInstance<MidBoss>()?.defaultDamage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(attackdamage.Value);
        }
    }
}
