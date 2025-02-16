using playerCharacter;
using System.Collections;
using Rework;
using UnityEngine;

public class Vine : BossAttack
{
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
            PlayerCharacter.Instance.TakeDamage(damage, true);
        }
    }
}
