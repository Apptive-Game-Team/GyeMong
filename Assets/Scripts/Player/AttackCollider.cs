using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public float attackDamage;
    private PlayerSoundController _soundController;
    private void Start()
    {
        var player = PlayerCharacter.Instance;
        attackDamage = player.attackPower;
    }

    public void Init(PlayerSoundController soundController)
    {
        _soundController = soundController;
    }
  
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var creature = collision.GetComponent<Creature>();
        if (creature != null)
        {
            _soundController.Trigger(PlayerSoundType.SWORD_ATTACK);
            creature.TakeDamage(attackDamage);
            Debug.Log("�ѱ��б�");
            Destroy(gameObject);
        } else
        {
            IAttackable[] attackableObjects = collision.GetComponents<IAttackable>();
            if (attackableObjects.Length != 0)
            {
                _soundController.Trigger(PlayerSoundType.SWORD_ATTACK);
                foreach (IAttackable @object in attackableObjects)
                {
                    @object.OnAttacked();
                }
            }
        }
    }
}
