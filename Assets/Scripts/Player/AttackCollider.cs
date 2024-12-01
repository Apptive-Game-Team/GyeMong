using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public float attackDamage;
    //Bad Way But..
    [SerializeField] private GameObject flowerObj;
    
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
            //Bad Way But..
            if (PlayerCharacter.Instance.GetComponent<RuneComponent>().isRune(3))
            {
                Debug.Log("Flower Rune Activated");
                Instantiate(flowerObj, collision.transform.position,quaternion.identity);
            }
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
