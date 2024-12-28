using playerCharacter;
using System.Collections;
using UnityEngine;

public class Vine : MonoBehaviour
{
    private float attackdamage = MidBoss.GetInstance<MidBoss>().defaultDamage;
    private void OnEnable()
    {
        StartCoroutine(ActivateVine());
    }
    private IEnumerator ActivateVine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(attackdamage, true);
        }
    }
}
