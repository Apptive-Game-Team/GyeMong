using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using Rework;
using UnityEngine;

public class Root : BossAttack
{
    new float damage = 10;
    private void OnEnable()
    {
        StartCoroutine(OffRootObjects());
    }
    private IEnumerator OffRootObjects()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
        }
    }
}
