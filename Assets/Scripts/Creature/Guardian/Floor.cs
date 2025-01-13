using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : GrazeController
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag("Player"))
        {
            isAttacked = true;
            PlayerCharacter.Instance.TakeDamage(10);
        }
    }
}
