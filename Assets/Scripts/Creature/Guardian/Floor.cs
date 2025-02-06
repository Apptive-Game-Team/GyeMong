using playerCharacter;
using Rework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : BossAttack
{
    float damage = 30;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
        }
    }
}
