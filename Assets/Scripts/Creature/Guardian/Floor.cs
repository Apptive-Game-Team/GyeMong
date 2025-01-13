using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : GrazeController
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacked = true;
            PlayerCharacter.Instance.TakeDamage(10);
        }
    }
}
