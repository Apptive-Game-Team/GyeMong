using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using Rework;
using UnityEngine;

public class Root : BossAttack
{
    private void OnEnable()
    {
        StartCoroutine(OffRootObjects());
    }
    private IEnumerator OffRootObjects()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(damage);
        }
    }
}
