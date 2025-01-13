using playerCharacter;
using System.Collections;
using UnityEngine;

public class Vine : GrazeController
{
    private float attackdamage;
    private void OnEnable()
    {
        attackdamage = Boss.GetInstance<MidBoss>().defaultDamage;
        StartCoroutine(ActivateVine());
    }
    private IEnumerator ActivateVine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        if (other.CompareTag("Player"))
        {
            isAttacked = true;
            PlayerCharacter.Instance.TakeDamage(attackdamage, true);
        }
    }
}
