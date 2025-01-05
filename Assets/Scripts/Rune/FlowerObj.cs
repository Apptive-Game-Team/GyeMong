using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

public class FlowerObj : MonoBehaviour
{
    public float attackDamage;
    public float damageCoef = 0.2f;
    public float duration = 0.5f;
    private void Start()
    {
        var player = PlayerCharacter.Instance;
        attackDamage = player.attackPower * damageCoef;
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var creature = collision.GetComponent<Creature>();
        if (creature != null)
        {
            creature.OnAttacked(attackDamage);
        }
    }
}
