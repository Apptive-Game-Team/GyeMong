using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAttack : MonoBehaviour
{
    public float? attackdamage = Boss.GetInstance<Guardian>()?.defaultDamage;
    private void Update()
    {
        if (Boss.GetInstance<Guardian>().curState == Creature.State.STUN)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(attackdamage.Value);
        }
    }
}