using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

public class MazeThorn : MonoBehaviour
{
    private float damage = 1f;
    private float damageDelay = 1f;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(Damage());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }


    private IEnumerator Damage()
    {
        while (true)
        {
            PlayerCharacter.Instance.TakeDamage(damage);
            yield return new WaitForSeconds(damageDelay);
        }
    }
}
