using playerCharacter;
using Rework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : BossAttack
{
    new float damage = 30;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
            Debug.Log("Ãæµ¹ÇÔ ¤·¤·");
        }
    }
}
