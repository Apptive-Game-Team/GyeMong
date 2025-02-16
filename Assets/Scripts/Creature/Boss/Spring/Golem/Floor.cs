using playerCharacter;
using Rework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : BossAttack
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        damage = 30;
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
            Debug.Log("asdfasdfasdf");
        }
    }
}
