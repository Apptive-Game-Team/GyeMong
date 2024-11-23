using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public float attackDamage;

    private void Start()
    {
        var player = PlayerCharacter.Instance;
        attackDamage = player.attackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var creature = collision.GetComponent<Creature>();
        if (creature != null)
        {
            creature.TakeDamage(attackDamage);
            Debug.Log("�ѱ��б�");

            Destroy(gameObject);
        } else
        {
            IAttackable[] attackableObjects = collision.GetComponents<IAttackable>();
            foreach (IAttackable @object in attackableObjects)
            {
                @object.OnAttacked();
            }
        }
    }
}
